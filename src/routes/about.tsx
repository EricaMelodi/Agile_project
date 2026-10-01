import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/about')({
  component: About,
})

function About() {
  return (
    <main className="page-wrap px-4 py-12">
      <section className="island-shell rounded-2xl p-6 sm:p-8">
        <p className="island-kicker mb-2">About</p>
        <h1 className="display-title mb-3 text-4xl font-bold text-[var(--sea-ink)] sm:text-5xl">
          A small starter with room to grow.
        </h1>
        <p className="m-0 max-w-3xl text-base leading-8 text-[var(--sea-ink-soft)]">
            <div className="max-w-3xl space-y-6 text-base leading-8 text-[var(--sea-ink-soft)]">

  <p>
    We are five students currently taking the course Software Project Management at Chalmers University of Technology. Our project focuses on Sustainable Development Goal 7: “Affordable and Clean Energy”. We wanted to create a project that helps people become more aware of electricity prices and their energy consumption.
  </p>

  <p>
    Our idea is an energy/saving website that shows when electricity prices are low or high and gives suggestions on how users can reduce their energy consumption when prices are high. With this project, we want to make it easier for people to make more sustainable choices in their everyday lives.
  </p>

  <p>
    Our slogan is “Every hour can be Earth Hour.” With this, we want to show that small actions throughout the day can make a difference. By using the website, we hope to encourage people to think more about when and how they use electricity and make smarter choices.
  </p>

  <p>
    During the project, we have learned more about teamwork, agile methods, scrum, user stories, sprint planning and APIs. We have also learned how important communication and having a realistic scope are when working on a software project.
  </p>

  <p>
    We strongly recommend taking this course because it gives students the opportunity to work on a real project and experience how software projects are managed in practice. It is a good way to learn about both teamwork and agile development.
  </p>

</div>
        </p>
      </section>
    </main>
  )
}
