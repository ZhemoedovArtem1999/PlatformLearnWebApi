import React from 'react';
import logo from './logo.svg';
import './App.css';
import { addLocale, PrimeReactProvider } from "primereact/api";
import { observer } from "mobx-react";
import { Header } from './components/common/Header/Header';
import ruLocale from "primelocale/ru.json";
import { Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import { Auth } from './components/pages/Auth/Auth';
import 'bootstrap/dist/css/bootstrap.min.css';
import { PlatformLearnApi } from './api';

import { basePlatformLearnUrl } from "./utils/constants";
import authStore from './stores/auth-store';



addLocale("ru", ruLocale["ru"]);


const App: React.FC = observer(() => {

  const locale = {
    locale: "ru",
  };


  return (
    <>
      <PrimeReactProvider value={locale}>
        <PlatformLearnApi apiAddress={basePlatformLearnUrl} token={authStore.accessToken || undefined}>
        <Router>
          <Header />
          <Routes>
            <Route path="/" element={<></>} />
            <Route path="/auth" element={<Auth />} />
          </Routes>
        </Router>
        </PlatformLearnApi>

      </PrimeReactProvider>



      {/* <div className="App">
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <p>
            Edit <code>src/App.tsx</code> and save to reload.
          </p>
          <a
            className="App-link"
            href="https://reactjs.org"
            target="_blank"
            rel="noopener noreferrer"
          >
            Learn React
          </a>
        </header>
      </div> */}
    </>
  );
}
);
export default App;
