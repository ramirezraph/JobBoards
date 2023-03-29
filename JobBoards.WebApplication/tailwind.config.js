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
  theme: {
    extend: {},
  },
  plugins: [],
}

