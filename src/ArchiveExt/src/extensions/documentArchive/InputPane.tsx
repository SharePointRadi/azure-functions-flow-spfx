import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Panel, PanelType } from 'office-ui-fabric-react/lib/Panel';
import { PrimaryButton } from 'office-ui-fabric-react/lib/Button';
import { HttpClient, IHttpClientOptions, HttpClientResponse } from '@microsoft/sp-http';
import { Dropdown, DropdownMenuItemType, IDropdownStyles, IDropdownOption } from 'office-ui-fabric-react/lib/Dropdown';
import { Stack } from 'office-ui-fabric-react/lib/Stack';

import { Label } from 'office-ui-fabric-react/lib/Label';
import { Dialog, DialogType } from 'office-ui-fabric-react/lib/Dialog';

interface IInputPaneContentProps {
    httpClient: HttpClient;
    hidden: boolean;
    itemId: number;
    spFilePath: string;
}

interface IInputPaneContentState {
    hidePanel: boolean;
    retention: string;
    confidentiality: string;
}
class InputPaneContent extends React.Component<IInputPaneContentProps, IInputPaneContentState> {
    constructor(props) {
        super(props);
        this.archiveFlow = this.archiveFlow.bind(this);
        this.archiveAzure = this.archiveAzure.bind(this);
        this.setRetention = this.setRetention.bind(this);
        this.setConfidentiality = this.setConfidentiality.bind(this);
        this.state = { hidePanel: this.props.hidden, retention: "", confidentiality: "" };
    }

    public render(): JSX.Element {

        const retentionOptions: IDropdownOption[] = [
            { key: 'retentionHeader', text: 'Years', itemType: DropdownMenuItemType.Header },
            { key: '1', text: '1' },
            { key: '3', text: '3' },
            { key: '5', text: '5' },
            { key: '10', text: '10' }
        ];

        const confidentialityOptions: IDropdownOption[] = [
            { key: 'Public', text: 'Public' },
            { key: 'Secret', text: 'Secret' }
        ];

        const dropdownStyles: Partial<IDropdownStyles> = {
            dropdown: { width: 200 }
        };

        let ok: boolean = this.state.retention !== "" && this.state.confidentiality !== "";
        return (<Panel isOpen={!this.state.hidePanel} type={PanelType.smallFixedFar}>
            <Stack gap={20}>
                <Stack.Item>
                    <Dropdown placeholder="Select an option"
                        label="Retention period"
                        options={retentionOptions}
                        styles={dropdownStyles}
                        onChanged={this.setRetention} />
                </Stack.Item>
                <Stack.Item>
                    <Dropdown placeholder="Select an option"
                        label="Confidentiality"
                        options={confidentialityOptions}
                        styles={dropdownStyles}
                        onChanged={this.setConfidentiality} />
                </Stack.Item>
                <Stack horizontal gap={20}>
                    <Stack.Item>
                        <PrimaryButton onClick={this.archiveFlow} text="Archive via Flow" disabled={!ok} />
                    </Stack.Item>
                    <Stack.Item>
                        <PrimaryButton onClick={this.archiveAzure} text="Archive via Azure" disabled={!ok} />
                    </Stack.Item>
                </Stack>
            </Stack>

        </Panel>);
    }

    private setRetention(option: IDropdownOption) {
        this.setState({ retention: option.text });
    }

    private setConfidentiality(option: IDropdownOption) {
        this.setState({ confidentiality: option.text });
    }

    private async archiveFlow() {
        this.setState({ hidePanel: true });
        const flowUrl = "https://prod-24.westeurope.logic.azure.com:443/workflows/5281b4b8534c4c749f7866e841126490/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=7X54ITuSxHoOGfFX6iintiPyCTnb7bDnr7j4WgQTEMA";

        const requestHeaders: Headers = new Headers();
        requestHeaders.append('Content-type', 'application/json');
        requestHeaders.append('Cache-Control', 'no-cache');

        const body: string = JSON.stringify({
            'Id': this.props.itemId,
            'RetentionPeriond': this.state.retention,
            'ConfidentialityLevel': this.state.confidentiality
        });

        const httpClientOptions: IHttpClientOptions = {
            body: body,
            headers: requestHeaders
        };
        let result = await this.props.httpClient.post(flowUrl, HttpClient.configurations.v1, httpClientOptions);
    }

    private async archiveAzure() {
        this.setState({ hidePanel: true });
        // TODO: Call using aadClient in a secure manner
        const azureUrl = "https://archive-vault.azurewebsites.net/api/ArchiveVault?code=z5c6qcR4aZEJHVmhOzN3YNC7ZKfH4V17qqRmvmV28LNxQA4ja4wBgw==";

        const requestHeaders: Headers = new Headers();
        requestHeaders.append('Content-type', 'application/json');
        requestHeaders.append('Cache-Control', 'no-cache');

        const body: string = JSON.stringify({
            "spFilePath": this.props.spFilePath,
            "confidentialityLevel": this.state.confidentiality,
            "retentionPeriod": this.state.retention
        });

        const httpClientOptions: IHttpClientOptions = {
            body: body,
            headers: requestHeaders
        };
        let result = await this.props.httpClient.post(azureUrl, HttpClient.configurations.v1, httpClientOptions);
    }
}

const div = document.createElement("div");

export default class InputPane {
    public hidden: boolean = true;
    private httpClient: HttpClient;
    private itemId: number;
    private spFilePath: string;

    constructor(props) {
        this.httpClient = props.httpClient;
        this.itemId = props.itemId;
        this.spFilePath = props.spFilePath;
        this.close = this.close.bind(this);
    }

    public render(): void {
        ReactDOM.render(<InputPaneContent
            httpClient={this.httpClient}
            hidden={this.hidden}
            itemId={this.itemId}
            spFilePath={this.spFilePath}
            key={"b" + new Date().toISOString()}
        />, div);
    }

    public show() {
        this.hidden = false;
        this.render();
    }

    public close() {
        this.hidden = true;
        this.render();
    }
}