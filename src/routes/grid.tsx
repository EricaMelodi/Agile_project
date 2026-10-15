import { useEffect, useRef, useState } from 'react'
import { createFileRoute } from '@tanstack/react-router'
import backgroundPicture from '../assets/BackgroundPic.jpeg'

type AddressFeature = {
  properties?: {
    formatted?: string
    address_line1?: string
    address_line2?: string
    lat?: number
    lon?: number
  }
}

type AddressSearchResponse = {
  features?: AddressFeature[]
  error?: string
}

type EnergyResponse = {
  zone?: string
  price?: {
    SEK_per_kWh?: number
    price_area?: string
  }
  cheapestHour?: PriceHour
  mostExpensiveHour?: PriceHour
  estimatedSavingPerUse?: number
  assumedConsumptionKWh?: number
  error?: string
}

type PriceHour = {
  timeStart: string
  timeEnd: string
  sekPerkWh: number
}

const energyApiUrl =
  import.meta.env.VITE_ENERGY_API_URL ?? 'http://localhost:5000'

export const Route = createFileRoute('/grid')({
  component: Grid,
})

function Grid() {
  const [query, setQuery] = useState('')
  const [suggestions, setSuggestions] = useState<AddressFeature[]>([])
  const [selectedAddress, setSelectedAddress] =
    useState<AddressFeature | null>(null)
  const [energy, setEnergy] = useState<EnergyResponse | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [isLoadingEnergy, setIsLoadingEnergy] = useState(false)
  const [error, setError] = useState('')
  const [energyError, setEnergyError] = useState('')

  const searchRequest = useRef<AbortController | null>(null)

  // rerendering automatic 

  useEffect(() => {
    const trimmedQuery = query.trim()

    setSelectedAddress(null)
    setEnergy(null)
    setError('')
    setEnergyError('')

    if (trimmedQuery.length < 3) {
      setSuggestions([])
      setIsLoading(false)
      return
    }

    const timeout = window.setTimeout(async () => {
      searchRequest.current?.abort()

      const controller = new AbortController()
      searchRequest.current = controller

      setIsLoading(true)

      try {
        const response = await fetch(
          `/api/address-search?q=${encodeURIComponent(trimmedQuery)}`,
          {
            signal: controller.signal,
          },
        )

        const data = (await response.json()) as AddressSearchResponse

        if (!response.ok) {
          throw new Error(data.error ?? 'Address search failed')
        }

        setSuggestions(data.features ?? [])
      } catch (requestError) {
        if (
          requestError instanceof DOMException &&
          requestError.name === 'AbortError'
        ) {
          return
        }

        console.error(requestError)
        setSuggestions([])
        setError('Could not load address suggestions.')
      } finally {
        if (!controller.signal.aborted) {
          setIsLoading(false)
        }
      }
    }, 300)

    return () => {
      window.clearTimeout(timeout)
    }
  }, [query])

  async function selectAddress(address: AddressFeature) {
    const formattedAddress = address.properties?.formatted

    if (!formattedAddress) {
      setEnergyError('The selected address is missing its address text.')
      return
    }

    setQuery(formattedAddress)
    setSelectedAddress(address)
    setSuggestions([])
    setEnergy(null)
    setEnergyError('')
    setIsLoadingEnergy(true)

    try {
      const response = await fetch(
        `${energyApiUrl}/api/current-energy?address=${encodeURIComponent(formattedAddress)}`,
      )
      const data = (await response.json()) as EnergyResponse

      if (!response.ok) {
        throw new Error(data.error ?? 'Could not load electricity price.')
      }

      setEnergy(data)
    } catch (requestError) {
      console.error(requestError)
      setEnergyError(
        'Could not load the electricity price for this address. Is the backend running?',
      )
    } finally {
      setIsLoadingEnergy(false)
    }
  }

  function formatHourRange(hour?: PriceHour) {
    if (!hour) return ''

    const start = new Date(hour.timeStart)
    const end = new Date(hour.timeEnd)

    return `${start.toLocaleTimeString('sv-SE', {
      hour: '2-digit',
      minute: '2-digit',
      timeZone: 'Europe/Stockholm',
    })}–${end.toLocaleTimeString('sv-SE', {
      hour: '2-digit',
      minute: '2-digit',
      timeZone: 'Europe/Stockholm',
    })}`
  }

  return (
    <main
      className="page-wrap min-h-screen bg-cover bg-center bg-fixed px-4 pb-16 pt-14"
      style={{ backgroundImage: `url(${backgroundPicture})` }}
    >
      <section className="rise-in mx-auto flex min-h-[31rem] max-w-3xl flex-col items-center justify-center rounded-[2rem] px-6 py-12 text-center sm:px-10">
        <p className="island-kicker mb-4 text-white">Today's grid</p>

        <h1 className="display-title mb-4 max-w-2xl text-4xl font-bold leading-[1.05] tracking-tight text-white sm:text-6xl">
          Find the prices at your location right now.
        </h1>

        <div className="relative w-full max-w-xl text-left">
          <label>
            <span className="sr-only">Your address</span>

            <input
              type="text"
              name="address"
              value={query}
              placeholder="Write your address"
              aria-label="Write your address"
              aria-autocomplete="list"
              aria-controls="address-suggestions"
              aria-expanded={suggestions.length > 0}
              onChange={(event) => setQuery(event.target.value)}
              className="h-16 w-full rounded-2xl border-2 border-white bg-white px-6 text-base text-[var(--sea-ink)] shadow-[0_14px_35px_rgba(0,0,0,0.14)] outline-none placeholder:text-[var(--sea-ink-soft)] focus:border-cyan-300 focus:ring-4 focus:ring-cyan-200/40"
            />
          </label>

          {(isLoading || suggestions.length > 0 || error) && (
            <div
              id="address-suggestions"
              className="absolute z-10 mt-2 w-full overflow-hidden rounded-2xl border border-white/70 bg-white text-left shadow-[0_14px_35px_rgba(0,0,0,0.2)]"
            >
              {isLoading && (
                <p className="px-5 py-4 text-sm text-[var(--sea-ink-soft)]">
                  Searching addresses...
                </p>
              )}

              {!isLoading && error && (
                <p className="px-5 py-4 text-sm text-red-700">{error}</p>
              )}

              {!isLoading && !error && suggestions.length > 0 && (
                <ul>
                  {suggestions.map((suggestion, index) => {
                    const formattedAddress =
                      suggestion.properties?.formatted ?? 'Unknown address'

                    return (
                      <li key={`${formattedAddress}-${index}`}>
                        <button
                          type="button"
                          onClick={() => selectAddress(suggestion)}
                          className="w-full px-5 py-4 text-left text-sm text-[var(--sea-ink)] transition hover:bg-[var(--sand)] focus:bg-[var(--sand)] focus:outline-none"
                        >
                          {formattedAddress}
                        </button>
                      </li>
                    )
                  })}
                </ul>
              )}
            </div>
          )}
        </div>

        {selectedAddress?.properties?.formatted && (
          <p className="mt-4 max-w-xl text-sm text-white">
            Selected address: {selectedAddress.properties.formatted}
          </p>
        )}

        {isLoadingEnergy && (
          <p className="mt-6 text-sm text-white">Loading current price...</p>
        )}

        {energyError && (
          <p className="mt-6 max-w-xl text-sm text-red-200">{energyError}</p>
        )}

        {energy?.price?.SEK_per_kWh !== undefined && (
          <div className="mt-6 w-full max-w-2xl space-y-4 text-[var(--sea-ink)]">
            <div className="rounded-2xl bg-white px-6 py-4 shadow-[0_14px_35px_rgba(0,0,0,0.14)]">
              <p className="text-sm font-semibold uppercase tracking-wide text-[var(--sea-ink-soft)]">
                Current electricity price
              </p>
              <p className="mt-1 text-3xl font-bold">
                {energy.price.SEK_per_kWh.toFixed(2)} SEK/kWh
              </p>
              {energy.zone && (
                <p className="mt-1 text-sm text-[var(--sea-ink-soft)]">
                  Price area: {energy.zone}
                </p>
              )}
            </div>

            {energy.cheapestHour && energy.mostExpensiveHour && (
              <div className="rounded-2xl bg-white px-6 py-5 text-left shadow-[0_14px_35px_rgba(0,0,0,0.14)]">
                <p className="text-sm font-semibold uppercase tracking-wide text-[var(--sea-ink-soft)]">
                  Todays prices - Most expensive hour VS cheapest hour
                </p>

                <div className="mt-4 grid gap-4 sm:grid-cols-2">
                  <div>
                    <p className="text-sm text-[var(--sea-ink-soft)]">
                      Most expensive hour
                    </p>
                    <p className="mt-1 text-xl font-bold">
                      {formatHourRange(energy.mostExpensiveHour)}
                    </p>
                    <p className="mt-1 text-sm">
                      {energy.mostExpensiveHour.sekPerkWh.toFixed(2)} SEK/kWh
                    </p>
                  </div>

                  <div className="sm:text-right">
                    <p className="text-sm text-emerald-600">
                      Cheapest hour - a tip, use this ☺
                    </p>
                    <p className="mt-1 text-xl font-bold">
                      {formatHourRange(energy.cheapestHour)}
                    </p>
                    <p className="mt-1 text-sm">
                      {energy.cheapestHour.sekPerkWh.toFixed(2)} SEK/kWh
                    </p>
                  </div>
                </div>

                <div className="mt-6 border-t border-[var(--line)] pt-4">
                  <p className="text-sm text-[var(--sea-ink-soft)]">
                    Estimated saving per use
                  </p>
                  <p className="mt-1 text-3xl font-bold text-emerald-600">
                    {(energy.estimatedSavingPerUse ?? 0).toFixed(2)} SEK
                  </p>
                  <p className="mt-2 text-sm text-[var(--sea-ink-soft)]">
                    Based on an assumed consumption of{' '}
                    {energy.assumedConsumptionKWh ?? 1} kWh and the difference
                    between the two hourly prices.
                  </p>
                </div>
              </div>
            )}
          </div>
        )}
      </section>
    </main>
  )
}