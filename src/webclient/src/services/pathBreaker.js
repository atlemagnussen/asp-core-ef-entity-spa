class PathBreaker {
    getRoute(p) {
        let route = {
            name: "/",
            param: ""
        };
        if (p === "/") {
            return route;
        }
        const slashes = p.match(/\//ig) || [];

        if (slashes.length === 0) {
            return route;
        } else if (slashes.length === 1) {
            route.name = p.replace("/", "");
            return route;
        }

        let pathSplit = p.split('/');

        pathSplit = this.clearEmptyElementsInArray(pathSplit);
        route.name = `${pathSplit.splice(0, 1)}`;
        route.param = `${pathSplit.splice(0, 1)}`;
        return route;
    };

    clearEmptyElementsInArray(arr) {
        for (let i = arr.length - 1; i >= 0; i--) {
            if (!arr[i]) {
                arr.splice(i, 1);
            }
        }
        return arr;
    }
}

export default new PathBreaker();