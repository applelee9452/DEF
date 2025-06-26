export function getTimezoneOffset() {
    return 0 - new Date().getTimezoneOffset() / 60;
}