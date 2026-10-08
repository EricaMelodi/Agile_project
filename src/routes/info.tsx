import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/info')({
  component: Info,
})

function Info() {
  return (
    <main className="page-wrap px-4 py-12">
      <section className="island-shell rounded-2xl p-6 sm:p-8">
        <p className="island-kicker mb-2">Info</p>
        <h1 className="display-title mb-8 text-4xl font-bold text-[var(--sea-ink)] sm:text-5xl">
          How does our website work?
        </h1>

        <div className="max-w-3xl space-y-6 text-base leading-8 text-[var(--sea-ink-soft)]">
          <p>
            Our website helps you understand electricity prices and make
            smarter choices when you use electricity.
          </p>

          <p>
            Enter your location to see the electricity prices for your area.
            The website will show you when the electricity is cheaper or more
            expensive, making it easier to decide when it may be a good time
            to use energy and when to not.
          </p>

          <p>
            When prices are high, we also provide simple suggestions for
            reducing your electricity consumption. For example, you might
            choose to wait with using certain appliances until the electricity
            price is lower.
          </p>

          <h2 className="pt-2 text-2xl font-bold text-[var(--sea-ink)]">
            Why use it?
          </h2>

          <p>
            Electricity prices can change throughout the day, and small
            changes in when we use electricity can make a difference. By being
            more aware of electricity prices, you can avoid unnecessary energy
            use during expensive periods and make more sustainable choices.
          </p>

          <p>
            By saving energy, you are not only reducing your electricity
            consumption. You are also helping reduce unnecessary energy
            production and its impact on the environment. Small changes in our
            everyday habits can contribute to a more sustainable future.
          </p>

          <p>
            The goal is not to change everything you do, but to make it easier
            to make small changes in your everyday life.
          </p>

          <p className="font-semibold text-[var(--sea-ink)]">
            Every hour can be Earth Hour.
          </p>
        </div>
      </section>
    </main>
  )
}
