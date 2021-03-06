import * as React from 'react';
import * as ReactDom from 'react-dom';
import { Version } from '@microsoft/sp-core-library';
import { BaseClientSideWebPart } from '@microsoft/sp-webpart-base';
import {
  IPropertyPaneConfiguration,
  PropertyPaneTextField
} from '@microsoft/sp-property-pane';

import * as strings from 'ArchiveVaultWebPartWebPartStrings';
import ArchiveVaultWebPart from './components/ArchiveVaultWebPart';
import { IArchiveVaultWebPartProps } from './components/IArchiveVaultWebPartProps';
import { HttpClient, AadHttpClient, HttpClientResponse } from '@microsoft/sp-http';


export interface IArchiveVaultWebPartWebPartProps {
  description: string;
}

export default class ArchiveVaultWebPartWebPart extends BaseClientSideWebPart<IArchiveVaultWebPartWebPartProps> {

  public render(): void {
    const element: React.ReactElement<IArchiveVaultWebPartProps> = React.createElement(
      ArchiveVaultWebPart,
      {
        description: this.properties.description,
        httpClient: this.context.httpClient,
        aadHttpClientFactory: this.context.aadHttpClientFactory
      }
    );

    ReactDom.render(element, this.domElement);
  }

  protected onDispose(): void {
    ReactDom.unmountComponentAtNode(this.domElement);
  }

  protected get dataVersion(): Version {
    return Version.parse('1.0');
  }

  protected getPropertyPaneConfiguration(): IPropertyPaneConfiguration {
    return {
      pages: [
        {
          header: {
            description: strings.PropertyPaneDescription
          },
          groups: [
            {
              groupName: strings.BasicGroupName,
              groupFields: [
                PropertyPaneTextField('description', {
                  label: strings.DescriptionFieldLabel
                })
              ]
            }
          ]
        }
      ]
    };
  }
}
