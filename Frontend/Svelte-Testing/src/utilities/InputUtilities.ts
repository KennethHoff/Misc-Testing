export function onlyAllowInputMatchingPattern(pattern: string, event: InputEvent): void {
	const input = event.target as HTMLInputElement;
	if (!pattern) {
		return;
	}

	const regex = new RegExp(pattern);
	const latestCharacter = input.value[input.value.length - 1];
	const latestCharacterIsValid = regex.test(latestCharacter);

	if (!latestCharacterIsValid) {
		input.value = input.value.slice(0, -1);
	}
}
