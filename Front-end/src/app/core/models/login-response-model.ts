export interface LoginResponseModel {
  token: string
  expiration: string
  refreshToken: string
  refreshTokenExpiration: string,
  userId: number,
  userName: string,
  email: string,
  role: string
}