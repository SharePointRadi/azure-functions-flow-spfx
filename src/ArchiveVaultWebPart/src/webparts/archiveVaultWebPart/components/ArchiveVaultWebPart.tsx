import * as React from 'react';
import styles from './ArchiveVaultWebPart.module.scss';
import { IArchiveVaultWebPartProps } from './IArchiveVaultWebPartProps';
import { escape } from '@microsoft/sp-lodash-subset';
import { HttpClient, HttpClientResponse } from '@microsoft/sp-http';

export default class ArchiveVaultWebPart extends React.Component<IArchiveVaultWebPartProps, any> {
  constructor(props: any) {
    super(props);

    this.state = {
      documents: []
    };

  }

  protected bindTable(data) {
    var allElements = [];
    data.map(document => {
      allElements.push(
        <tr>
          <td><a href={document.SpFilePath}>{document.FileName}</a></td>
          <td>{document.ConfidentialityLevel}</td>
          <td>{document.RetentionPeriod} years</td>
        </tr>)
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
              <table className="coolTable">
                <thead className="tableStyle-thead">
                  <td>File Name</td>
                  <td>Confidentiality Level</td>
                  <td>Retention Period</td>
                </thead>
                <tbody>
                  {
                    this.bindTable(this.state.documents)
                  }
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    );
  }

  public componentDidMount(): void {
    var functionUrl = "https://archive-vault.azurewebsites.net/api/GetArchiveVaultDocuments?code=LDhSQkGDY9OBim1KYqDaZ9SQrs6J9eIYjDiWna/ISqgFBFJT8sz1Qg==";

    this.props.httpClient.get(functionUrl, HttpClient.configurations.v1)
      .then((response: HttpClientResponse) => {
        if (response.ok) {


          response.json().then(data => {
            this.setState({
              documents: data,
            });
            console.log(data);
          }
          );





        } else {
          Promise.resolve(null);
        }
      });
  }
}
