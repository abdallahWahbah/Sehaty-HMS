export interface LoginResponseModel {
  userName: string,
  email: string,
  token: string,
  expiration: string,
  refreshToken: string,
  refreshTokenExpiration: string,
  userId: number,
  role: string
}