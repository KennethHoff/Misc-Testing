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
                background: colors.zinc[800],
                action: {
                    DEFAULT: colors.zinc[600],
                    hover: colors.zinc[400],
                },
                focus: {
                    DEFAULT: colors.zinc[100],
                },
                invalid: {
                    DEFAULT: colors.red[800],
                    hover: colors.red[600],
                },
            },
            // outlineStyle: {
            //     dashed: 'dashed',
            // }
        },
        outlineStyle: {
            dashed: 'dashed',
        }
    },
    plugins: [
        // Aria-invalid
        function ({ addVariant, e }) {
            addVariant('aria-invalid', ({ modifySelectors, separator }) => {
                modifySelectors(({ className }) => {
                    return `.${e(`aria-invalid${separator}${className}`)}[aria-invalid]`
                })
            })
        },
    ],
}

