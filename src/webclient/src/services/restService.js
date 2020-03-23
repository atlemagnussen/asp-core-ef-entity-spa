import { userProfile } from "../store";

class RestService {
    constructor() {
        this.upData = {};
        userProfile.subscribe(val => {
            this.upData = val;
        });
    }
    async getWithAuth(url) {
        const res = await fetch(url, {
            method: "GET",
            headers: {
                'Authorization': 'Bearer ' + this.upData.access_token
            }
        });
        if (res.ok) {
            const json = await res.json();
			return json;
        }
        await this.errorHandler(res);
    }
    async postWithAuth(url, data) {
        const res = await fetch(url, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.upData.access_token
            },
            body: JSON.stringify(data)
        });
        if (res.ok) {
            const json = await res.json();
			return json;
        }
        return await this.errorHandler(res);
    }
    async errorHandler(res) {
        if (res.status === 401) {
            throw new Error("401 you need to authenticate");
        }
        if (res.status === 403) {
            throw new Error("403 you are not authorized with the right role");
        }
        var text = await res.text();
        throw new Error(text);
    }
}

export default new RestService();