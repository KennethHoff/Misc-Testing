/** @type {import('tailwindcss').Config} */
import colors from "tailwindcss/colors";

module.exports = {
    content: [
        "./Pages/**/*.razor",
        "./Components/**/*.razor",
        "wwwroot/**/*.js",
    ],
    // TODO: Make the design more consistent
    theme: {
        colors: {
            transparent: 'transparent',
            background: {
                DEFAULT: colors.zinc[800],
            },
            primary: {
                DEFAULT: colors.zinc[100],
            },
            action: {
                DEFAULT: colors.blue[500],
                active: colors.blue[400],
            },
            focus: {
                DEFAULT: colors.zinc[100],
            },
            invalid: {
                DEFAULT: colors.red[800],
                active: colors.red[600],
            },
            valid: {
                DEFAULT: colors.green[800],
                active: colors.green[600],
            },
            // TODO: Get a better name for this concept
            table: {
                odd: colors.zinc[800],
                even: colors.zinc[700],
            },
            hover: {
                DEFAULT: colors.zinc[500],
            }
        },
        extend: {
            gridTemplateColumns: {
                'comment-table': '125px 1fr 150px'
            },
        },
        outlineStyle: {
            dashed: 'dashed',
        }
    },
    plugins: [
        // Aria-invalid
        function ({addVariant, e}) {
            addVariant('aria-invalid', ({modifySelectors, separator}) => {
                modifySelectors(({className}) => {
                    return `.${e(`aria-invalid${separator}${className}`)}[aria-invalid]`
                })
            })
        },
    ],
}

