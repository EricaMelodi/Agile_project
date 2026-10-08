import { createFileRoute } from '@tanstack/react-router'
import backgroundPicture from '../assets/BackgroundPic.jpeg'

export const Route = createFileRoute('/grid')({ component: Grid })

function Grid() {
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

        <label className="w-full max-w-xl text-left">
          <span className="sr-only">Your address</span>
          <input
            type="text"
            name="address"
            placeholder="Write your address"
            aria-label="Write your address"
            className="h-16 w-full rounded-2xl border-2 border-white bg-white px-6 text-base text-[var(--sea-ink)] shadow-[0_14px_35px_rgba(0,0,0,0.14)] outline-none placeholder:text-[var(--sea-ink-soft)] focus:border-cyan-300 focus:ring-4 focus:ring-cyan-200/40"
          />
        </label>
      </section>
    </main>
  )
}
