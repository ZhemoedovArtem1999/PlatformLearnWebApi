import React, { useEffect } from "react";
import logo from "./logo.svg";
import "./App.css";
import { addLocale, PrimeReactProvider } from "primereact/api";
import { observer } from "mobx-react";
import { Header } from "./components/common/Header/Header";
import ruLocale from "primelocale/ru.json";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import { Auth } from "./components/pages/Auth/Auth";
import "bootstrap/dist/css/bootstrap.min.css";
import { PlatformLearnApi } from "./api";

import { basePlatformLearnUrl } from "./utils/constants";
import authStore from "./stores/auth-store";
import { AuthServiceClientApi } from "../src/api/grpc/client/auth.client";
import { TokenValidRequest } from "./api/grpc/api/auth/auth_pb";
import * as grpcWeb from 'grpc-web';

addLocale("ru", ruLocale["ru"]);



const App: React.FC = observer(() => {
  const locale = {
    locale: "ru",
  };

  useEffect(() => {
    const validateToken = async () => {
      try {
        if (authStore.accessToken) {
          AuthServiceClientApi.addMetadata("Authorization", `Bearer ${authStore.accessToken}`);
          await AuthServiceClientApi.TokenValid(new TokenValidRequest());
        }
      } catch (err) {
        if (err instanceof grpcWeb.RpcError && err.code === 16) {
          authStore.clearAuth();
        }
      }
    };

    validateToken();
  }, []);

  return (
    <>
      <PrimeReactProvider value={locale}>
        <PlatformLearnApi
          apiAddress={basePlatformLearnUrl}
          token={authStore.accessToken || undefined}
        >
          <Router>
            <Header />
            <Routes>
              <Route path="/" element={<></>} />
              <Route path="/auth" element={<Auth />} />
            </Routes>
          </Router>
        </PlatformLearnApi>
      </PrimeReactProvider>
    </>
  );
});
export default App;
