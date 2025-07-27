import { InputText } from "primereact/inputtext";
import React, { useState } from "react";
import styles from "./Auth.module.css";
import { Logo } from "../../common/Header/Logo";
import { Button } from "primereact/button";
import { TestAuthApi, usePlatformLearnApi } from "../../../api";
import { AuthServiceClientApi, IRegisterRequest } from "../../../api/grpc/client/auth.client";
import { nameof } from "ts-simple-nameof";
import { Calendar } from "primereact/calendar";
import { Dropdown } from "primereact/dropdown";
import { RegisterRequest } from "../../../api/grpc/api/auth/auth_pb";
import { Timestamp } from 'google-protobuf/google/protobuf/timestamp_pb';


interface IGender {
    name: string;
    value: string;
}

export const Register: React.FC = () => {
    const testApi = usePlatformLearnApi(TestAuthApi);

    const [registerData, setRegisterData] = useState<IRegisterRequest>();

    const gender: IGender[] = [{ name: "Мужской", value: "Мужской" }, { name: "Женский", value: "Женский" }]
    console.log(registerData);
    const handleChange = (propName: string, value: any) => {
        setRegisterData({ ...registerData, ...{ [propName]: value } } as IRegisterRequest);
    };

    const handleRegistration = async () => {
        console.log("Работаем ");
        const request = new RegisterRequest();
        request.setLastname(registerData.lastname);
        request.setFirstname(registerData.firstname);
        request.setMiddlename(registerData.middlename);
        if (registerData.date_birth) {
            const timestamp = new Timestamp();
            timestamp.fromDate(new Date(registerData.date_birth));
            request.setDateBirth(timestamp);
        }
        request.setGender(registerData.gender);
        request.setEmail(registerData.email);
        request.setPassword(registerData.password);
        request.setPassword2(registerData.password2);


        try {
            const response = await AuthServiceClientApi.Register(request);
            console.log("Register success:", response);
            window.location.href = '/auth';

        } catch (err) {
            console.error("Register failed:", err);
        }
    };

    const handleLogin = async () => {
        // console.log();
        // const request = new LoginRequest();
        // request.setLogin(email);
        // request.setPassword(password);

        // try {
        //     const response = await AuthServiceClientApi.Login(request);
        //     console.log("Login success:", response);

        //     authStore.setPlatformLearnAuth(response.token, response.username);
        // } catch (err) {
        //     console.error("Login failed:", err);
        // }
    };

    return (
        <>
            <div
                style={{
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                    height: "100vh",
                    margin: 0,
                }}
            >
                <div
                    style={{
                        width: "400px",
                        minHeight: "100px",
                        backgroundColor: "#07a9e9ff",
                        borderRadius: "25px",
                    }}
                >
                    <div className={styles.loginInputs}>
                        <div>
                            <Logo />
                        </div>
                        <p style={{ color: "white", fontSize: 25, margin: "10px" }}>
                            Регистрация
                        </p>

                        <InputText
                            id={nameof<IRegisterRequest>((_) => _.lastname)}
                            title="Введите фамилию"
                            value={registerData?.lastname ?? ''}
                            onChange={e => handleChange(nameof<IRegisterRequest>((_) => _.lastname), e.target.value)}
                            placeholder="Введите фамилию"
                            className="inputField"
                            pt={{
                                root: {
                                    style: {
                                        width: "90%",
                                    },
                                },
                            }}
                        />
                        <InputText
                            id={nameof<IRegisterRequest>((_) => _.firstname)}
                            title="Введите имя"
                            value={registerData?.firstname ?? ''}
                            onChange={e => handleChange(nameof<IRegisterRequest>((_) => _.firstname), e.target.value)}
                            placeholder="Введите имя"
                            className="inputField"
                            pt={{
                                root: {
                                    style: {
                                        width: "90%",
                                    },
                                },
                            }}
                        />
                        <InputText
                            id={nameof<IRegisterRequest>((_) => _.middlename)}
                            title="Введите отчетво"
                            value={registerData?.middlename ?? ''}
                            onChange={e => handleChange(nameof<IRegisterRequest>((_) => _.middlename), e.target.value)}
                            placeholder="Введите отчетво"
                            className="inputField"
                            pt={{
                                root: {
                                    style: {
                                        width: "90%",
                                    },
                                },
                            }}
                        />
                        <Calendar
                            id={nameof<IRegisterRequest>((_) => _.date_birth)}
                            value={registerData?.date_birth}
                            onChange={e => handleChange(nameof<IRegisterRequest>((_) => _.date_birth), e.target.value)}
                            placeholder="Введите дату рождения"
                            showIcon={true}
                            locale="ru"

                            pt={{
                                root: {
                                    style: {
                                        width: "90%",
                                    },
                                },
                                panel: {
                                    style: {
                                        backgroundColor: "white",
                                        color: 'black',
                                        border: '1px solid white'
                                    }
                                },
                            }}
                        />

                        <Dropdown
                            id={nameof<IRegisterRequest>((_) => _.gender)}
                            style={{ backgroundColor: "white" }}
                            title="Выберите пол"
                            placeholder="Выберите пол"
                            value={registerData?.gender}
                            options={gender}
                            onChange={(e) => handleChange(nameof<IRegisterRequest>((_) => _.gender), e.value)}
                            optionLabel="name"
                            className="inputField"
                            pt={{
                                root: {
                                    style: {
                                        width: "90%",
                                    },
                                },
                                panel: {
                                    style: {
                                        backgroundColor: "white",
                                        color: 'black',
                                        border: '1px solid white'
                                    }
                                },

                                item: {
                                    style: {
                                        backgroundColor: "white",
                                        color: "black",

                                        "&:hover": {
                                            backgroundColor: "#07a9e9",
                                            color: "white"
                                        },

                                        "&.p-highlight": {
                                            backgroundColor: "#f0f0f0",
                                            color: "black"
                                        }
                                    }
                                }
                            }}
                        />
                        <InputText
                            id={nameof<IRegisterRequest>((_) => _.email)}
                            title="Введите почту"
                            value={registerData?.email ?? ''}
                            onChange={e => handleChange(nameof<IRegisterRequest>((_) => _.email), e.target.value)}
                            placeholder="Введите почту"
                            className="inputField"
                            pt={{
                                root: {
                                    style: {
                                        width: "90%",
                                    },
                                },
                            }}
                        />
                        <InputText
                            id={nameof<IRegisterRequest>((_) => _.password)}
                            title="Введите пароль"
                            value={registerData?.password ?? ''}
                            placeholder="Пароль"
                            onChange={e => handleChange(nameof<IRegisterRequest>((_) => _.password), e.target.value)}
                            type="password"
                            className="inputField"
                            pt={{
                                root: {
                                    style: {
                                        width: "90%",
                                    },
                                },
                            }}
                        />
                        <InputText
                            id={nameof<IRegisterRequest>((_) => _.password2)}
                            title="Повторите пароль"
                            value={registerData?.password2 ?? ''}
                            placeholder="Пароль"
                            onChange={e => handleChange(nameof<IRegisterRequest>((_) => _.password2), e.target.value)}
                            type="password"
                            className="inputField"
                            pt={{
                                root: {
                                    style: {
                                        width: "90%",
                                    },
                                },
                            }}
                        />
                        <Button
                            className="buttonCustom"
                            label="Зарегистрироваться"
                            style={{ width: "90%" }}
                            onClick={handleRegistration}
                        />
                    </div>
                </div>
            </div>
        </>
    );
};
