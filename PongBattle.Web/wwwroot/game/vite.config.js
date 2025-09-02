import { defineConfig } from 'vite';
import { svelte } from '@sveltejs/vite-plugin-svelte';

export default defineConfig({
  plugins: [svelte()],
  build: {
    outDir: '../js',
    rollupOptions: {
      input: 'src/main.js',
      output: {
        entryFileNames: 'game-bundle.js',
        format: 'iife'
      }
    }
  }
});