/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        '!**/{bin,obj,node_modules}/**',
        '**/*.{cshtml,html}'
    ],
    prefix: 'tw-',
    corePlugins: {
        preflight: false,
    },
    darkMode: false, // or 'media' or 'class'
    theme: {
        extend: {},
    },
    variants: {
        extend: {},
    },
    plugins: [],
}

