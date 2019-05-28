import * as React from "react";
import styles from "./ArchiveVaultWebPart.module.scss";
import { IArchiveVaultWebPartProps } from "./IArchiveVaultWebPartProps";
import { escape } from "@microsoft/sp-lodash-subset";
import {
  HttpClient,
  AadHttpClient,
  HttpClientResponse
} from "@microsoft/sp-http";
import { Environment, EnvironmentType } from "@microsoft/sp-core-library";

export default class ArchiveVaultWebPart extends React.Component<IArchiveVaultWebPartProps, any> {
  constructor(props: any) {
    super(props);

    this.state = {
      documents: []
    };
  }

  protected bindTable(data) {
    var allElements = [];
    data.map((document, index) => {
      allElements.push(
        <tr key={index}>
          <td>
            <a href={document.SpFilePath}>{document.FileName}</a>
          </td>
          <td>{document.ConfidentialityLevel}</td>
          <td>{document.RetentionPeriod} years</td>
        </tr>
      );
    });
    return allElements;
  }

  public render(): React.ReactElement<IArchiveVaultWebPartProps> {
    return (
      <div className={styles.archiveVaultWebPart}>
        <div className={styles.container}>
          <div className={styles.row}>
            <div className={styles.column}>
              <span className={styles.title}>Archive Vault</span>
              <div>&nbsp;</div>
              <table className={styles.archiveTable}>
                <thead className={styles.tableStylethead}>
                  <tr>
                    <td>File Name</td>
                    <td>Confidentiality Level</td>
                    <td>Retention Period</td>
                  </tr>
                </thead>
                <tbody>{this.bindTable(this.state.documents)}</tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    );
  }

  public componentDidMount(): void {
    if (Environment.type == EnvironmentType.ClassicSharePoint) {
      this.callWithHttpClient();
      //Classic SharePoint page
    } else if (Environment.type === EnvironmentType.Local) {
      this.callWithHttpClient();
      //Workbenck page
    } else if (Environment.type === EnvironmentType.SharePoint) {
      this.callWithHttpClient();
      //Modern SharePoint page
    } else if (Environment.type === EnvironmentType.Test) {
      this.callWithHttpClient();
      //Running on Unit test enveironment
    }
  }

  private callWithHttpClient(): void {
    var functionUrl =
      "https://archive-vault.azurewebsites.net/api/GetArchiveVaultDocuments?code=LDhSQkGDY9OBim1KYqDaZ9SQrs6J9eIYjDiWna/ISqgFBFJT8sz1Qg==";

    this.props.httpClient
      .get(functionUrl, HttpClient.configurations.v1)
      .then((response: HttpClientResponse) => {
        if (response.ok) {
          response.json().then(data => {
            this.setState({
              documents: data
            });
            console.log(data);
          });
        } else {
          Promise.resolve(null);
        }
      });
  }

  private callWithAadHttpClient(): void {
    this.props.aadHttpClientFactory
      .getClient("https://archive-vault.azurewebsites.net")
      .then(
        (client: AadHttpClient): void => {
          client
            .get(
              "https://archive-vault.azurewebsites.net/api/GetArchiveVaultDocuments",
              AadHttpClient.configurations.v1
            )
            .then((response: HttpClientResponse) => {
              return response.json();
            })
            .then(
              (data): void => {
                // process data
              }
            );
        }
      );
  }
}
