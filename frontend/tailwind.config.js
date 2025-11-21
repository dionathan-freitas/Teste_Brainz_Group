/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'elegant-purple': '#6366f1',
        'elegant-purple-dark': '#4f46e5',
        'elegant-gold': '#f59e0b',
        'elegant-silver': '#e5e7eb',
      },
    },
  },
  plugins: [require('daisyui')],
  daisyui: {
    themes: [
      {
        elegant: {
          'primary': '#6366f1',
          'primary-focus': '#4f46e5',
          'primary-content': '#ffffff',
          'secondary': '#8b5cf6',
          'secondary-focus': '#7c3aed',
          'secondary-content': '#ffffff',
          'accent': '#f59e0b',
          'accent-focus': '#d97706',
          'accent-content': '#ffffff',
          'neutral': '#1f2937',
          'neutral-focus': '#111827',
          'neutral-content': '#ffffff',
          'base-100': '#ffffff',
          'base-200': '#f3f4f6',
          'base-300': '#e5e7eb',
          'base-content': '#1f2937',
          'info': '#3b82f6',
          'success': '#10b981',
          'warning': '#f59e0b',
          'error': '#ef4444',
        },
      },
    ],
    base: true,
    styled: true,
    utils: true,
  },
}
