export function setupCustomValidationPatterns() {
    const ssnInputElement = document.querySelector('#cvp_ssnInput');
    checkValidityOfInputs();
    ssnInputElement.addEventListener("input", checkValidityOfInputs);
    function checkValidityOfInputs() {
        const ssnInputValue = ssnInputElement.value;
        console.log("SSN: ", ssnInputValue);
        let length = ssnInputValue.toString().length;
        if (length !== 11) {
            ssnInputElement.setCustomValidity("SSN must be 11 digits. Currently " + length + " digits.");
            return;
        }
        if (length % 11 !== 0) {
            ssnInputElement.setCustomValidity("SSN must be 11 digits and end with a check digit.(ie, be a multiple of 11)");
            return;
        }
    }
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3VzdG9tVmFsaWRhdGlvblBhdHRlcm5zLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiY3VzdG9tVmFsaWRhdGlvblBhdHRlcm5zLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBLE1BQU0sVUFBVSw2QkFBNkI7SUFFNUMsTUFBTSxlQUFlLEdBQUcsUUFBUSxDQUFDLGFBQWEsQ0FBbUIsZUFBZSxDQUFFLENBQUM7SUFFbkYscUJBQXFCLEVBQUUsQ0FBQztJQUN4QixlQUFlLENBQUMsZ0JBQWdCLENBQUMsT0FBTyxFQUFFLHFCQUFxQixDQUFDLENBQUM7SUFFakUsU0FBUyxxQkFBcUI7UUFFN0IsTUFBTSxhQUFhLEdBQUcsZUFBZSxDQUFDLEtBQUssQ0FBQztRQUM1QyxPQUFPLENBQUMsR0FBRyxDQUFDLE9BQU8sRUFBRSxhQUFhLENBQUMsQ0FBQztRQUNwQyxJQUFJLE1BQU0sR0FBRyxhQUFhLENBQUMsUUFBUSxFQUFFLENBQUMsTUFBTSxDQUFDO1FBRTdDLElBQUksTUFBTSxLQUFLLEVBQUUsRUFDakI7WUFDQyxlQUFlLENBQUMsaUJBQWlCLENBQUMsbUNBQW1DLEdBQUcsTUFBTSxHQUFHLFVBQVUsQ0FBQyxDQUFDO1lBQzdGLE9BQU87U0FDUDtRQUVELElBQUksTUFBTSxHQUFHLEVBQUUsS0FBSyxDQUFDLEVBQ3JCO1lBQ0MsZUFBZSxDQUFDLGlCQUFpQixDQUFDLDRFQUE0RSxDQUFDLENBQUM7WUFDaEgsT0FBTztTQUNQO0lBQ0YsQ0FBQztBQUNGLENBQUMifQ==