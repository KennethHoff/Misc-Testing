import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'

// https://vitejs.dev/config/
export default defineConfig({
	plugins: [
		svelte(),
	],
	build:{
		// generate manifest.json in outDir
		manifest: true,
		rollupOptions: {
			// overwrite default .html entry
			input: 'src/entries/main.ts',
		},
		outDir: '../wwwroot/dist/'
	},
	server: {
		host: 'localhost',
		port: 3000,
		https: true,
		open: true,
		proxy:{
			'*' : {
				target: 'https://localhost:7188/',
				changeOrigin: true
			}
		},
		hmr: {
			host: 'localhost',
		}
	}
})