import React, { createContext, ReactNode, useContext, useMemo, useReducer, useState } from "react";
import * as ApiTypes from "./api/data-contracts";
import { ApiConfig, HttpClient } from "./api/http-client";

type UserCredentials = Record<string, string>;

interface LoginErrorCallback {
  (error: unknown): void;
}

function SetPlatformLearnCredentialsReducer(
  oldCredentials: UserCredentials | undefined,
  newCredentials?: UserCredentials | null,
): UserCredentials | undefined {
  if (newCredentials) return newCredentials;

  if (oldCredentials) return undefined;

  return oldCredentials;
}

function AddPlatformLearnLoginErrorCallbackReducer(
  callbacks: LoginErrorCallback[],
  newCallback?: LoginErrorCallback,
): LoginErrorCallback[] {
  if (newCallback) {
    if (!callbacks.find((c) => c == newCallback)) {
      return [...callbacks, newCallback];
    }
  }

  return callbacks;
}

const PlatformLearnAddLoginErrorCallbackContext = createContext<React.Dispatch<LoginErrorCallback>>(() => undefined);
const PlatformLearnApiConfigContext = createContext<ApiConfig<ApiTypes.AuthentcationResponseDto> | undefined>(
  undefined,
);
const PlatformLearnAuthorizationContext = createContext<
  [ApiTypes.AuthentcationResponseDto | undefined, React.Dispatch<UserCredentials | undefined | null>]
>([undefined, () => void 0]);

const usePlatformLearnAuthorization = () => useContext(PlatformLearnAuthorizationContext);
const usePlatformLearnApiConfig = () => useContext(PlatformLearnApiConfigContext);
const usePlatformLearnAddLoginErrorCallback = () => useContext(PlatformLearnAddLoginErrorCallbackContext);

//TODO: переделать с хука на обычную функцию. Подумать над использованием useMemo (учесть что при отмене операции будут отменятся все запросы к данному апи).
function usePlatformLearnApi<ApiType extends HttpClient<unknown>>(
  apiType: new (apiConfig: ApiConfig<unknown>) => ApiType,
  apiConfig: ApiConfig<ApiTypes.AuthentcationResponseDto> = {},
): ApiType | undefined {
  const [authorization] = usePlatformLearnAuthorization();
  const baseConfig = usePlatformLearnApiConfig();

  const api = new apiType({
    ...baseConfig,
    ...apiConfig,
  } as ApiConfig<unknown>);

  if (authorization) api.setSecurityData(authorization);
  return api;
}

const PlatformLearnApi = ({
  apiAddress,
  credentials,
  onLoginError,
  token,
  children,
}: {
  apiAddress: string;
  credentials?: UserCredentials;
  onLoginError?: LoginErrorCallback;
  token?: string;
  children: ReactNode;
}) => {
  const [authorizationData, setAuthorizationData] = useState<ApiTypes.AuthentcationResponseDto | undefined>(
    token ? { token: token } : undefined,
  );
  const [apiCredentials, setApiCredentials] = useReducer(SetPlatformLearnCredentialsReducer, credentials);
  const [callbacks, addLoginErrorCallback] = useReducer(
    AddPlatformLearnLoginErrorCallbackReducer,
    onLoginError ? [onLoginError] : [],
  );

  const apiConfig = useMemo(() => {
    return {
      baseUrl: apiAddress,
      baseApiParams: {
        secure: true,
      },
      securityWorker: (authorizationData: ApiTypes.AuthentcationResponseDto | null) => {
        if (authorizationData) {
          return { headers: { authorization: `Bearer ${authorizationData.token}` } };
        }
      },
    };
  }, [apiAddress]);

  return (
    <PlatformLearnApiConfigContext.Provider value={apiConfig}>
      <PlatformLearnAddLoginErrorCallbackContext.Provider value={addLoginErrorCallback}>
        <PlatformLearnAuthorizationContext.Provider value={[authorizationData, setApiCredentials]}>
          {children}
        </PlatformLearnAuthorizationContext.Provider>
      </PlatformLearnAddLoginErrorCallbackContext.Provider>
    </PlatformLearnApiConfigContext.Provider>
  );
};

export {
  PlatformLearnApi,
  ApiTypes as PlatformLearnApiTypes,
  usePlatformLearnAddLoginErrorCallback,
  usePlatformLearnApi,
  usePlatformLearnAuthorization,
};
export type { LoginErrorCallback, UserCredentials };

export { AuthApi } from "./api/AuthApi";
export { TestAuthApi } from "./api/TestAuthApi";
