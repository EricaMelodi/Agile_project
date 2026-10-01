import { createFileRoute } from '@tanstack/react-router'
import backgroundPicture from '../assets/BackgroundPic.jpeg'

export const Route = createFileRoute('/')({ component: App })

function App() {
  return (
    <div style={{ backgroundImage: `url(${backgroundPicture})` }}>
      <main
        className="page-wrap min-h-screen bg-cover bg-center bg-fixed px-4 pb-8 pt-14"
        style={{ backgroundImage: `url(${backgroundPicture})` }}
      >
        <section className=" rise-in relative overflow-hidden rounded-[2rem] px-6 py-10 sm:px-10 sm:py-14">

          <h1 className="display-title mb-5 max-w-3xl text-center text-4xl leading-[1.02] font-bold tracking-tight text-white sm:text-5xl">
            Every hour
          </h1>
          <h1 className="display-title mb-5 max-w-3xl text-center text-4xl leading-[1.02] font-bold tracking-tight text-[var(--sea-ink)] sm:text-5xl bg-gradient-to-r from-teal-600 via-cyan-400 to-slate-50 bg-clip-text text-transparent">
            can be Earth Hour.
          </h1>
          <p className="mb-8 max-w-2xl text-center text-gray-400 sm:text-lg">
            Our Project watches the electricity grid so you don't have to.
            Know the exact hour to run your washing machine, dishwasher, or charge your car.
            When energy is cheapest and cleanest.
          </p>
          <div className="flex flex-wrap items-center justify-center gap-4">
            <a
              href="/about"
              className="inline-flex items-center gap-2 rounded-full bg-cyan-400 px-6 py-3 text-sm font-bold text-black shadow-[0_0_20px_rgba(34,211,238,0.3)] transition hover:bg-cyan-300"
            >
              See Today's Grid
              <span aria-hidden="true">&rarr;</span>
            </a>

            <a
              href="https://tanstack.com/router"
              target="_blank"
              rel="noopener noreferrer"
              className="rounded-full border border-slate-700 bg-slate-900/40 px-6 py-3 text-sm font-semibold text-slate-300 transition hover:border-slate-500 hover:text-white"
            >
              About us
            </a>
          </div>
        </section>

        <section className="mt-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          {[
            [
              'Type-Safe Routing',
              'Routes and links stay in sync across every page.',
            ],
            [
              'Server Functions',
              'Call server code from your UI without creating API boilerplate.',
            ],
            [
              'Streaming by Default',
              'Ship progressively rendered responses for faster experiences.',
            ],
            [
              'Tailwind Native',
              'Design quickly with utility-first styling and reusable tokens.',
            ],
          ].map(([title, desc], index) => (
            <article
              key={title}
              className="island-shell feature-card rise-in rounded-2xl p-5"
              style={{ animationDelay: `${index * 90 + 80}ms` }}
            >
              <h2 className="mb-2 text-base font-semibold text-[var(--sea-ink)]">
                {title}
              </h2>
              <p className="m-0 text-sm text-[var(--sea-ink-soft)]">{desc}</p>
            </article>
          ))}
        </section>

        <section className="island-shell mt-8 rounded-2xl p-6">
          <p className="island-kicker mb-2">Quick Start</p>
          <ul className="m-0 list-disc space-y-2 pl-5 text-sm text-[var(--sea-ink-soft)]">
            <li>
              Edit <code>src/routes/index.tsx</code> to customize the home page.
            </li>
            <li>
              Update <code>src/components/Header.tsx</code> and{' '}
              <code>src/components/Footer.tsx</code> for brand links.
            </li>
            <li>
              Add routes in <code>src/routes</code> and tweak visual tokens in{' '}
              <code>src/styles.css</code>.
            </li>
          </ul>
        </section>
      </main>
    </div>
  )
}
