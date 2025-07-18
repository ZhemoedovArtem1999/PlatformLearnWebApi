import { InputText } from "primereact/inputtext";
import React, { useState } from "react";
import styles from "./Auth.module.css"
import { Logo } from "../../common/Header/Logo";
import { Button } from "primereact/button";
import { AuthApi, TestAuthApi, usePlatformLearnApi } from "../../../api";
import authStore from "../../../stores/auth-store";

export const Auth: React.FC = () => {
    const authApi = usePlatformLearnApi(AuthApi);
    const testApi = usePlatformLearnApi(TestAuthApi);

    const [email, setEmail] = useState<string>();
    const [password, setPassword] = useState<string>();


    const handleRegistration = () => {
        console.log("Проверка");
        testApi?.login();
        // window.location.href = '/';
    };

    const handleLogin = async () => {
        await authApi?.login({ email: email, password: password })
            .then((result) => {
                console.log(result);
                authStore.setPlatformLearnAuth(result.token!, result.username!);
            })
            .catch(e => {
            })
    };

    return (
        <>
            <div style={{ display: "flex", justifyContent: "center", alignItems: "center", height: "100vh", margin: 0 }}>
                <div style={{ width: "400px", minHeight: "100px", backgroundColor: "#07a9e9ff", borderRadius: "25px" }}>
                    <div className={styles.loginInputs}>
                        <div>
                            <Logo />
                        </div>
                        <p style={{ color: "white", fontSize: 25, margin: "10px" }}>Вход в профиль</p>

                        <InputText
                            id='login'
                            title="Почта"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="Почта"
                            className="inputField"
                            pt={{
                                root: {
                                    style: {
                                        width: '90%',
                                    }
                                }
                            }}

                        />
                        <InputText
                            id='password'
                            title="Пароль"
                            value={password}
                            placeholder="Пароль"
                            onChange={(e) => setPassword(e.target.value)}
                            type="password"
                            className="inputField"
                            pt={{
                                root: {
                                    style: {
                                        width: '90%'
                                    }
                                }
                            }}
                        />

                        <p><a style={{ color: "white" }} href="/">Забыли пароль?</a></p>
                        <Button className="buttonCustom" label="Войти" style={{ width: "90%" }} onClick={handleLogin} />

                        <Button className="buttonCustom" label="Создать аккаунт" style={{ width: "90%" }} onClick={handleRegistration} />
                    </div>
                </div>

            </div>

        </>
    );

};
