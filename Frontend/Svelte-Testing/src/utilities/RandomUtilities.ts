// Generate random 10-digit number
export function GenerateRandomNumber(length: number): string {
	return Math.floor(Math.random() * 10 ** length).toString();
}

export function GenerateRandomId(prefix?: string): string {
	return `${prefix || ""}${GenerateRandomNumber(10)}`;
}
