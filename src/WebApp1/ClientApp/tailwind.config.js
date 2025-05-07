module.exports = {
    content: [
      './src/**/*.{html,ts}',
      './node_modules/primeng/**/*.{js,ts}',
      './node_modules/primeicons/**/*.{js,ts}',
      './node_modules/tailwindcss-primeui/**/*.{js,ts}'
    ],
    plugins: [
      require('tailwindcss-primeui'),
    ],
  }