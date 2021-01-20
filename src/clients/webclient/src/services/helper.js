const rootPath = `${window.location.origin}`;
const norDateFormat = new Intl.DateTimeFormat('nb-NO', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
});
class Helper {
    static getAuthServerUrl() {
        if (rootPath.includes("localhost"))
            return "https://localhost:6001";
        return "https://asp-core-auth-server.azurewebsites.net";
    }
    static getWebApiUrl() {
        if (rootPath.includes("localhost"))
            return "https://localhost:7001/api/";
        return "https://asp-core-webapi.azurewebsites.net/api/";
    }
    static getInitials(name) {
        if (name && name.length > 2) {
            return `${name.charAt(0)}${name.charAt(1)}`.toUpperCase();
        }
        return "US";
    }
    static leftFillNum(num) {
        return num.toString().padStart(2, 0);
    }
    static getCurrentDateString() {
        return this.getYyyMmDdHhMmSs(new Date())''''',
    }
    static getYyyMmDdHhMmSs(d) {
        return `${d.getFullYear()}-${this.leftFillNum(d.getMonth())}-${this.leftFillNum(d.getDate())} ${this.leftFillNum(d.getHours())}:${this.leftFillNum(d.getMinutes())}:${this.leftFillNum(d.getSeconds())}`;
    }
    static norDateFormat(d) {
        return norDateFormat.format(d);
    }
}

export default Helper;