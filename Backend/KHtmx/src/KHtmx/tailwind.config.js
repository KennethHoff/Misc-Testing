/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
      "./Pages/**/*.razor",
      "./Components/**/*.razor"
  ],
  theme: {
    extend: {
        gridTemplateColumns: {
            'comment-table': '125px 1fr 150px'
        }
    },
  },
  plugins: [],
}

