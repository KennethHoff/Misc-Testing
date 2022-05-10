module.exports = {
	root: true,
	parser: '@typescript-eslint/parser',
	plugins: [
		'@typescript-eslint',
	],
	parserOptions: {
		ecmaVersion: 'latest',
	},
	env: {
		browser: true,
	},
	extends: [
		'eslint:recommended',
		'plugin:tailwind/recommended',
		'plugin:@typescript-eslint/recommended',
	],
	rules: {}
}