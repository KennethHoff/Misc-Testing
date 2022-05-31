export function setupCrossInputValidation() {
    const phoneInputElement = document.querySelector('#civ_phoneInput');
    const emailInputElement = document.querySelector("#civ_emailInput");
    const submitButtonElement = document.querySelector(".civ_submit");
    checkValidityOfInputs();
    phoneInputElement.addEventListener("input", checkValidityOfInputs);
    emailInputElement.addEventListener("input", checkValidityOfInputs);
    submitButtonElement.addEventListener("submit", e => e.preventDefault());
    function checkValidityOfInputs() {
        const phoneInputValue = phoneInputElement.value;
        emailInputElement.required = phoneInputValue.length === 5;
    }
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3Jvc3NJbnB1dFZhbGlkYXRpb24uanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJjcm9zc0lucHV0VmFsaWRhdGlvbi50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxNQUFNLFVBQVUseUJBQXlCO0lBRXhDLE1BQU0saUJBQWlCLEdBQUcsUUFBUSxDQUFDLGFBQWEsQ0FBbUIsaUJBQWlCLENBQUUsQ0FBQztJQUN2RixNQUFNLGlCQUFpQixHQUFHLFFBQVEsQ0FBQyxhQUFhLENBQW1CLGlCQUFpQixDQUFFLENBQUM7SUFDdkYsTUFBTSxtQkFBbUIsR0FBRyxRQUFRLENBQUMsYUFBYSxDQUFvQixhQUFhLENBQUUsQ0FBQztJQUV0RixxQkFBcUIsRUFBRSxDQUFDO0lBRXhCLGlCQUFpQixDQUFDLGdCQUFnQixDQUFDLE9BQU8sRUFBRSxxQkFBcUIsQ0FBQyxDQUFDO0lBQ25FLGlCQUFpQixDQUFDLGdCQUFnQixDQUFDLE9BQU8sRUFBRSxxQkFBcUIsQ0FBQyxDQUFDO0lBQ25FLG1CQUFtQixDQUFDLGdCQUFnQixDQUFDLFFBQVEsRUFBRSxDQUFDLENBQUMsRUFBRSxDQUFDLENBQUMsQ0FBQyxjQUFjLEVBQUUsQ0FBQyxDQUFDO0lBRXhFLFNBQVMscUJBQXFCO1FBRTdCLE1BQU0sZUFBZSxHQUFHLGlCQUFpQixDQUFDLEtBQUssQ0FBQztRQUVoRCxpQkFBaUIsQ0FBQyxRQUFRLEdBQUcsZUFBZSxDQUFDLE1BQU0sS0FBSyxDQUFDLENBQUM7SUFDM0QsQ0FBQztBQUNGLENBQUMifQ==