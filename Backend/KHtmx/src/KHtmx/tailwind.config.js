/** @type {import('tailwindcss').Config} */
import colors from "tailwindcss/colors";

module.exports = {
    content: [
        "./Pages/**/*.razor",
        "./Components/**/*.razor"
    ],
    theme: {
        extend: {
            gridTemplateColumns: {
                'comment-table': '125px 1fr 150px'
            },
            colors: {
                'background': colors.zinc["800"],
                action: {
                    DEFAULT: colors.zinc[600],
                    hover: colors.zinc[500],
                }
            }
        },
    },
    plugins: [],
}

