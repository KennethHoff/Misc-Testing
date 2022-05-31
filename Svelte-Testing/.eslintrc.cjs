module.exports = {
	root: true,
	parser: '@typescript-eslint/parser',
	plugins: [
		'@typescript-eslint',
	],
	parserOptions: {
		ecmaVersion: '2022',
	},
	env: {
		browser: true,
	},
	extends: [
		'eslint:recommended',
		'plugin:tailwind/recommended',
		'plugin:@typescript-eslint/recommended',
	],
	"overrides": [
		{
			"files": ["*.svelte"],
			"parser": "svelte-eslint-parser"
		}
	],
	rules: {
		"no-mixed-spaces-and-tabs": ["error", "smart-tabs"],
	}
}