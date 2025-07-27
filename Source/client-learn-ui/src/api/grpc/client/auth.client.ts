
import { AuthServiceClient } from '../api/auth/auth_grpc_web_pb.js';
import * as grpcWeb from 'grpc-web';
import * as api from '../api/auth/auth_pb.js';
const GATEWAY_HOST = process.env.REACT_APP_GATEWAY_HOST + '/auth';

export interface ITokenValidRequest {

}

export interface ITokenValidResponse {
  success: boolean;
}

export interface ILoginRequest {
  login: string;
  password: string;
}

export interface ILoginResponse {
  token: string;
  username: string;
  success: boolean;
  message: string;
}

export interface IRegisterRequest {
  email?: string;
  password?: string;
  password2?: string;
  lastname?: string;
  firstname?: string;
  middlename?: string;
  date_birth?: Date;
  gender?: string;
}

export interface IRegisterResponse {
  success: boolean;
  message: string;
}

export class AuthServiceService {
  private client: AuthServiceClient;
  private metadata: { [key: string]: string } = {};



  public Login = async (request: api.LoginRequest): Promise<ILoginResponse> => {
    try {
      const response = await this.LoginInternal(request);
      return response as unknown as ILoginResponse;
    }
    catch (error) {
      if (error instanceof grpcWeb.RpcError) {
        throw error;
      }
      else {
        throw error;
      }
    }

  };

  private LoginInternal = (request: api.LoginRequest): Promise<api.LoginResponse> => {
    return this.callMethod('login', request);
  };

  public Register = async (request: api.RegisterRequest): Promise<IRegisterResponse> => {
    try {
      const response = await this.RegisterInternal(request);
      return response as unknown as IRegisterResponse;
    }
    catch (error) {
      if (error instanceof grpcWeb.RpcError) {
        throw error;
      }
      else {
        throw error;
      }
    }

  };

  private RegisterInternal = (request: api.RegisterRequest): Promise<api.RegisterResponse> => {
    return this.callMethod('register', request);
  };

  public TokenValid = async (request: api.TokenValidRequest): Promise<ITokenValidResponse> => {
    try {
      const response = await this.TokenValidInternal(request);
      return response as unknown as ITokenValidResponse;
    }
    catch (error) {
      if (error instanceof grpcWeb.RpcError) {
        throw error;
      }
      else {
        throw error;
      }
    }

  };

  private TokenValidInternal = (request: api.TokenValidRequest): Promise<api.TokenValidResponse> => {
    return this.callMethod('tokenValid', request);
  };

  constructor() {
    this.client = new AuthServiceClient(GATEWAY_HOST, null, {
      format: 'binary',
      withCredentials: false,
      debug: process.env.NODE_ENV === 'development'
    });
  }

  setMetadata(metadata: { [key: string]: string }) {
    this.metadata = { ...metadata };
  }

  addMetadata(key: string, value: string) {
    this.metadata[key] = value;
  }

  getMetadata(): grpcWeb.Metadata {
    const metadata: grpcWeb.Metadata = {};
    Object.entries(this.metadata).forEach(([key, value]) => {
      metadata[key] = value;
    });
    return metadata;
  }

  clearMetadata() {
    this.metadata = {};
  }

  private async callMethod<TRequest, TResponse>(
    methodName: string,
    request: TRequest
  ): Promise<TResponse> {
    return new Promise((resolve, reject) => {
      const method = (this.client as any)[methodName];
      if (!method) {
        reject(new Error(`Method ${methodName} not found`));
        return;
      }

      method.call(
        this.client,
        request,
        this.getMetadata(),
        (err: grpcWeb.RpcError, response: TResponse) => {
          if (err) {
            reject(err);
          } else {
            resolve(
              typeof (response as any).toObject === 'function'
                ? (response as any).toObject()
                : response
            );
          }
        }
      );
    });
  }
}

export const AuthServiceClientApi = new AuthServiceService();
