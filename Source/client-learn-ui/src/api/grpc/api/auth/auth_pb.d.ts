import * as jspb from 'google-protobuf'

import * as google_protobuf_timestamp_pb from 'google-protobuf/google/protobuf/timestamp_pb'; // proto import: "google/protobuf/timestamp.proto"


export class LoginRequest extends jspb.Message {
  getLogin(): string;
  setLogin(value: string): LoginRequest;

  getPassword(): string;
  setPassword(value: string): LoginRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): LoginRequest.AsObject;
  static toObject(includeInstance: boolean, msg: LoginRequest): LoginRequest.AsObject;
  static serializeBinaryToWriter(message: LoginRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): LoginRequest;
  static deserializeBinaryFromReader(message: LoginRequest, reader: jspb.BinaryReader): LoginRequest;
}

export namespace LoginRequest {
  export type AsObject = {
    login: string,
    password: string,
  }
}

export class LoginResponse extends jspb.Message {
  getToken(): string;
  setToken(value: string): LoginResponse;

  getUsername(): string;
  setUsername(value: string): LoginResponse;

  getSuccess(): boolean;
  setSuccess(value: boolean): LoginResponse;

  getMessage(): string;
  setMessage(value: string): LoginResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): LoginResponse.AsObject;
  static toObject(includeInstance: boolean, msg: LoginResponse): LoginResponse.AsObject;
  static serializeBinaryToWriter(message: LoginResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): LoginResponse;
  static deserializeBinaryFromReader(message: LoginResponse, reader: jspb.BinaryReader): LoginResponse;
}

export namespace LoginResponse {
  export type AsObject = {
    token: string,
    username: string,
    success: boolean,
    message: string,
  }
}

export class RegisterRequest extends jspb.Message {
  getEmail(): string;
  setEmail(value: string): RegisterRequest;

  getPassword(): string;
  setPassword(value: string): RegisterRequest;

  getPassword2(): string;
  setPassword2(value: string): RegisterRequest;

  getLastname(): string;
  setLastname(value: string): RegisterRequest;

  getFirstname(): string;
  setFirstname(value: string): RegisterRequest;

  getMiddlename(): string;
  setMiddlename(value: string): RegisterRequest;

  getDateBirth(): google_protobuf_timestamp_pb.Timestamp | undefined;
  setDateBirth(value?: google_protobuf_timestamp_pb.Timestamp): RegisterRequest;
  hasDateBirth(): boolean;
  clearDateBirth(): RegisterRequest;

  getGender(): string;
  setGender(value: string): RegisterRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): RegisterRequest.AsObject;
  static toObject(includeInstance: boolean, msg: RegisterRequest): RegisterRequest.AsObject;
  static serializeBinaryToWriter(message: RegisterRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): RegisterRequest;
  static deserializeBinaryFromReader(message: RegisterRequest, reader: jspb.BinaryReader): RegisterRequest;
}

export namespace RegisterRequest {
  export type AsObject = {
    email: string,
    password: string,
    password2: string,
    lastname: string,
    firstname: string,
    middlename: string,
    dateBirth?: google_protobuf_timestamp_pb.Timestamp.AsObject,
    gender: string,
  }
}

export class RegisterResponse extends jspb.Message {
  getSuccess(): boolean;
  setSuccess(value: boolean): RegisterResponse;

  getMessage(): string;
  setMessage(value: string): RegisterResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): RegisterResponse.AsObject;
  static toObject(includeInstance: boolean, msg: RegisterResponse): RegisterResponse.AsObject;
  static serializeBinaryToWriter(message: RegisterResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): RegisterResponse;
  static deserializeBinaryFromReader(message: RegisterResponse, reader: jspb.BinaryReader): RegisterResponse;
}

export namespace RegisterResponse {
  export type AsObject = {
    success: boolean,
    message: string,
  }
}

