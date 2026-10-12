import { createFileRoute } from '@tanstack/react-router'
import { json } from '@tanstack/react-start'

export const Route = createFileRoute('/api/address-search')({
  server: {
    handlers: {
      GET: async ({ request }) => {
        const query = new URL(request.url).searchParams
          .get('q')
          ?.trim()

        if (!query || query.length < 3) {
          return json({ features: [] })
        }

        const apiKey = process.env.GEOAPIFY_API_KEY

        if (!apiKey) {
          console.error('GEOAPIFY_API_KEY is not configured')

          return json(
            { error: 'Address search is not configured' },
            { status: 500 },
          )
        }

        const geoapifyUrl = new URL(
          'https://api.geoapify.com/v1/geocode/autocomplete',
        )

        geoapifyUrl.searchParams.set('text', query)
        geoapifyUrl.searchParams.set('filter', 'countrycode:se')
        geoapifyUrl.searchParams.set('lang', 'sv')
        geoapifyUrl.searchParams.set('limit', '5')
        geoapifyUrl.searchParams.set('apiKey', apiKey)

        const response = await fetch(geoapifyUrl)

        if (!response.ok) {
          console.error(
            `Geoapify address search failed with status ${response.status}`,
          )

          return json(
            { error: 'Address search failed' },
            { status: 502 },
          )
        }

        const data = await response.json()

        return json({
          features: data.features ?? [],
        })
      },
    },
  },
})