export type RecommendationGoal = 'cheap' | 'clean' | 'balanced'
export type RecommendationActivity = 'ev' | 'appliance'

export type PriceInterval = {
  time_start: string
  SEK_per_kWh: number
}

export type CarbonInterval = {
  datetime: string
  carbonIntensity: number
}

export type Recommendation = {
  start: Date
  end: Date
  averagePrice: number
  averageCarbon: number | null
  savedKronor: number
  avoidedCarbon: number | null
}

const intervalMinutes = 15

function getCarbonForTime(
  time: Date,
  carbonForecast: CarbonInterval[],
): number | null {
  if (carbonForecast.length === 0) return null

  let closest: CarbonInterval | undefined
  let closestDifference = Number.POSITIVE_INFINITY

  for (const entry of carbonForecast) {
    const difference = Math.abs(new Date(entry.datetime).getTime() - time.getTime())
    if (difference < closestDifference) {
      closest = entry
      closestDifference = difference
    }
  }

  // A distant value is more misleading than an unavailable value.
  return closest && closestDifference <= 90 * 60 * 1000
    ? closest.carbonIntensity
    : null
}

function windowValues(
  prices: PriceInterval[],
  carbonForecast: CarbonInterval[],
  windowSize: number,
) {
  return prices.slice(0, prices.length - windowSize + 1).flatMap((_, index) => {
    const entries = prices.slice(index, index + windowSize)
    const start = new Date(entries[0].time_start)
    const isContinuous = entries.every((entry, entryIndex) => {
      if (entryIndex === 0) return true
      const previous = new Date(entries[entryIndex - 1].time_start)
      return new Date(entry.time_start).getTime() - previous.getTime() === intervalMinutes * 60 * 1000
    })

    if (!isContinuous) return []

    const carbonValues = entries
      .map((entry) => getCarbonForTime(new Date(entry.time_start), carbonForecast))
      .filter((value): value is number => value !== null)

    return [{
      start,
      end: new Date(start.getTime() + windowSize * intervalMinutes * 60 * 1000),
      averagePrice: entries.reduce((sum, entry) => sum + entry.SEK_per_kWh, 0) / entries.length,
      averageCarbon: carbonValues.length === entries.length
        ? carbonValues.reduce((sum, value) => sum + value, 0) / carbonValues.length
        : null,
    }]
  })
}

function normalize(value: number, minimum: number, maximum: number): number {
  return maximum === minimum ? 0 : (value - minimum) / (maximum - minimum)
}

export function findRecommendation(
  prices: PriceInterval[],
  carbonForecast: CarbonInterval[],
  goal: RecommendationGoal,
  activity: RecommendationActivity,
): Recommendation | null {
  const windowSize = activity === 'ev' ? 16 : 8
  const consumptionKwh = activity === 'ev' ? 20 : 1.5
  const windows = windowValues(
    [...prices].sort((a, b) => a.time_start.localeCompare(b.time_start)),
    carbonForecast,
    windowSize,
  )

  if (windows.length === 0) return null

  const pricesOnly = windows.map((window) => window.averagePrice)
  const priceMinimum = Math.min(...pricesOnly)
  const priceMaximum = Math.max(...pricesOnly)
  const carbonWindows = windows.filter(
    (window): window is typeof window & { averageCarbon: number } =>
      window.averageCarbon !== null,
  )
  const carbonValues = carbonWindows.map((window) => window.averageCarbon)
  const carbonMinimum = carbonValues.length > 0 ? Math.min(...carbonValues) : 0
  const carbonMaximum = carbonValues.length > 0 ? Math.max(...carbonValues) : 0

  const scored = windows.map((window) => {
    const priceScore = normalize(window.averagePrice, priceMinimum, priceMaximum)
    const carbonScore = window.averageCarbon === null
      ? Number.POSITIVE_INFINITY
      : normalize(window.averageCarbon, carbonMinimum, carbonMaximum)

    const score = goal === 'cheap'
      ? priceScore
      : goal === 'clean'
        ? carbonScore
        : carbonScore === Number.POSITIVE_INFINITY
          ? priceScore
          : (priceScore + carbonScore) / 2

    return { window, score }
  })

  const available = scored.filter(({ score }) => Number.isFinite(score))
  if (available.length === 0) return null

  const selected = available.reduce((best, current) =>
    current.score < best.score ? current : best,
  ).window
  const peakPrice = Math.max(...pricesOnly)
  const savedKronor = Math.max(0, (peakPrice - selected.averagePrice) * consumptionKwh)
  const peakCarbon = carbonValues.length > 0 ? Math.max(...carbonValues) : null
  const avoidedCarbon = selected.averageCarbon !== null && peakCarbon !== null
    ? Math.max(0, (peakCarbon - selected.averageCarbon) * consumptionKwh)
    : null

  return {
    ...selected,
    savedKronor,
    avoidedCarbon,
  }
}
