import React from "react";
import { Menubar } from "primereact/menubar";
import { MenuItem } from "primereact/menuitem";
import { Logo } from "./Logo";
import authStore from "../../../stores/auth-store";

interface IHeaderProps {
  item?: string;
}

export const Header: React.FC<IHeaderProps> = (props: IHeaderProps) => {

  const logout = () => {
    authStore.clearAuth();
  };

  const items: MenuItem[] = [
    {
      label: "Поддержка",
      url: "/",
      className: "menu-item",
    },
    {
      label: "Каталог",
      items: [
        {
          label: "Программирование",
          url: "/",
          className: "menu-item",
        },
        {
          label: "Дизайн",
          url: "/",
          className: "menu-item",
        },
      ],
      className: "menu-item",
    },
    {
      label: "Войти",
      url: "/auth",
      className: "menu-item",
      visible: !authStore.isAuth(),
    },
    {
      label: "Выйти",
      command: logout,
      className: "menu-item",
      visible: authStore.isAuth(),
    },
  ];
  return (
    <div className="header-wrapper">
      <div
        className="header-content"
        style={{ display: "flex", justifyContent: "space-between" }}
      >
        <Logo />
        <Menubar
          model={items}
          className="custom-menubar"
          style={{ display: "flex", justifyContent: "flex-end" }}
        />
      </div>
    </div>
  );
};

export default Header;
