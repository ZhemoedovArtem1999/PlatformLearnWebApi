import * as grpcWeb from 'grpc-web';

import * as auth_pb from './auth_pb'; // proto import: "auth.proto"


export class AuthServiceClient {
  constructor (hostname: string,
               credentials?: null | { [index: string]: string; },
               options?: null | { [index: string]: any; });

  login(
    request: auth_pb.LoginRequest,
    metadata: grpcWeb.Metadata | undefined,
    callback: (err: grpcWeb.RpcError,
               response: auth_pb.LoginResponse) => void
  ): grpcWeb.ClientReadableStream<auth_pb.LoginResponse>;

  register(
    request: auth_pb.RegisterRequest,
    metadata: grpcWeb.Metadata | undefined,
    callback: (err: grpcWeb.RpcError,
               response: auth_pb.RegisterResponse) => void
  ): grpcWeb.ClientReadableStream<auth_pb.RegisterResponse>;

}

export class AuthServicePromiseClient {
  constructor (hostname: string,
               credentials?: null | { [index: string]: string; },
               options?: null | { [index: string]: any; });

  login(
    request: auth_pb.LoginRequest,
    metadata?: grpcWeb.Metadata
  ): Promise<auth_pb.LoginResponse>;

  register(
    request: auth_pb.RegisterRequest,
    metadata?: grpcWeb.Metadata
  ): Promise<auth_pb.RegisterResponse>;

}

