import { makeAutoObservable, action } from "mobx";

class AuthStore {
    accessToken: string | null = null;
    userName: string | null = null;
    errorMessage: string | null = null;
    isLoading: boolean = false;

    constructor() {
        makeAutoObservable(this);
        this.loadFromLocalStorage();
    }

    // Сохраняет данные авторизации в localStorage
    @action
    saveToLocalStorage() {
        localStorage.setItem(
            "auth",
            JSON.stringify({
                accessToken: this.accessToken,
                userName: this.userName,
            })
        );
    }

    // Загружает данные авторизации из localStorage
    @action
    loadFromLocalStorage() {
        const savedAuth = localStorage.getItem("auth");
        if (savedAuth) {
            const { accessToken, userName, rsmSpaAccessToken } = JSON.parse(savedAuth);
            this.accessToken = accessToken;
            this.userName = userName;
        }
    }
    @action
    setPlatformLearnAuth(token: string, name: string) {
        this.accessToken = token;
        this.userName = name;
        this.saveToLocalStorage(); // Сохраняем данные в localStorage
    }

    // Очищает токен и имя пользователя
    @action
    clearAuth() {
        this.accessToken = null;
        this.userName = null;
        localStorage.removeItem("auth");
    }


}

const authStore = new AuthStore();
export default authStore;
