import { HttpClient, AadHttpClientFactory, HttpClientResponse } from '@microsoft/sp-http';
export interface IArchiveVaultWebPartProps {
  description: string;
  httpClient: HttpClient;
  aadHttpClientFactory: AadHttpClientFactory;
}
