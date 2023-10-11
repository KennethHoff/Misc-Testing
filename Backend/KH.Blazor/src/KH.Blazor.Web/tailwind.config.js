/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./Components/**/*.razor"],
  theme: {
    extend: {
        backgroundImage: utils => ({
            'icon_hamburger': "url('/img/icon_hamburger.svg')",
        }),
    },
  },
  plugins: [],
}

