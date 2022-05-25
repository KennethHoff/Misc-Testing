export function setupCrossInputValidation()
{
	const phoneInputElement = document.querySelector<HTMLInputElement>('#civ_phoneInput')!;
	const emailInputElement = document.querySelector<HTMLInputElement>("#civ_emailInput")!;
	const submitButtonElement = document.querySelector<HTMLButtonElement>(".civ_submit")!;

	checkValidityOfInputs();

	phoneInputElement.addEventListener("input", checkValidityOfInputs);
	emailInputElement.addEventListener("input", checkValidityOfInputs);
	submitButtonElement.addEventListener("submit", e => e.preventDefault());

	function checkValidityOfInputs()
	{
		const phoneInputValue = phoneInputElement.value;

		emailInputElement.required = phoneInputValue.length === 5;
	}
}
