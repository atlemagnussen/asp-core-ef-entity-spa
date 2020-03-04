class Helper {
    getInitials(name) {
        if (name && name.length > 2) {
            return `${name.charAt(0)}${name.charAt(1)}`.toUpperCase();
        }
        return "US";
    }
}

export default new Helper();