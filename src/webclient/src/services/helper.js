class Helper {
    getInitials(name) {
        if (name && name.length > 2) {
            return `${name.charAt(0)}${name.charAt(1)}`.toUpperCase();
        }
        return "US";
    }
    leftFillNum(num) {
        return num.toString().padStart(2, 0);
    }
    getCurrentDateString() {
        return this.getYyyMmDdHhMmSs(new Date())
    }
    getYyyMmDdHhMmSs(d) {
        return `${d.getFullYear()}-${this.leftFillNum(d.getMonth())}-${this.leftFillNum(d.getDate())} ${this.leftFillNum(d.getHours())}:${this.leftFillNum(d.getMinutes())}:${this.leftFillNum(d.getSeconds())}`;
    }
}

export default new Helper();