export function setupCustomValidationPatterns()
{
	const ssnInputElement = document.querySelector<HTMLInputElement>('#cvp_ssnInput')!;

	checkValidityOfInputs();
	ssnInputElement.addEventListener("input", checkValidityOfInputs);

	function checkValidityOfInputs()
	{
		const ssnInputValue = ssnInputElement.value;
		console.log("SSN: ", ssnInputValue);
		let length = ssnInputValue.toString().length;

		if (length !== 11)
		{
			ssnInputElement.setCustomValidity("SSN must be 11 digits. Currently " + length + " digits.");
			return;
		}

		if (length % 11 !== 0)
		{
			ssnInputElement.setCustomValidity("SSN must be 11 digits and end with a check digit.(ie, be a multiple of 11)");
			return;
		}
	}
}