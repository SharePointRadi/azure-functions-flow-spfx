import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Panel, PanelType } from 'office-ui-fabric-react/lib/Panel';
import { PrimaryButton } from 'office-ui-fabric-react/lib/Button';
import { HttpClient, IHttpClientOptions, HttpClientResponse } from '@microsoft/sp-http';
import { Label } from 'office-ui-fabric-react/lib/Label';
import { Dialog, DialogType } from 'office-ui-fabric-react/lib/Dialog';

interface IInputPaneContentProps {
    httpClient: HttpClient;
    closeCallback: () => void;
    hidden: boolean;
    itemId: number;
}

interface IInputPaneContentState {
    hidePanel: boolean;
}
class InputPaneContent extends React.Component<IInputPaneContentProps, IInputPaneContentState> {
    constructor(props) {
        super(props);
        this.archive = this.archive.bind(this);
        this.state = { hidePanel: this.props.hidden };
    }

    public render(): JSX.Element {

        return (<Panel isOpen={!this.state.hidePanel} type={PanelType.smallFixedFar}>
            <PrimaryButton onClick={this.archive} text="Archive" />
        </Panel>);
    }

    private async archive() {
        this.setState({ hidePanel: true });
        const flowUrl = "https://prod-24.westeurope.logic.azure.com:443/workflows/5281b4b8534c4c749f7866e841126490/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=7X54ITuSxHoOGfFX6iintiPyCTnb7bDnr7j4WgQTEMA";

        const requestHeaders: Headers = new Headers();
        requestHeaders.append('Content-type', 'application/json');
        requestHeaders.append('Cache-Control', 'no-cache');

        const body: string = JSON.stringify({
            'Id': this.props.itemId,
            'RetentionPeriond': 5,
            'ConfidentialityLevel': "Public"
        });

        const httpClientOptions: IHttpClientOptions = {
            body: body,
            headers: requestHeaders
        };
        let result = await this.props.httpClient.post(flowUrl, HttpClient.configurations.v1, httpClientOptions);

        // if (this.props.closeCallback) {
        //     this.props.closeCallback();
        // }
    }
}

const div = document.createElement("div");

export default class InputPane {
    public hidden: boolean = true;
    private httpClient: HttpClient;
    private itemId: number;

    constructor(props) {
        this.httpClient = props.httpClient;
        this.itemId = props.itemId;
        this.close = this.close.bind(this);
    }

    public render(): void {
        ReactDOM.render(<InputPaneContent
            httpClient={this.httpClient}
            closeCallback={this.close}
            hidden={this.hidden}
            itemId={this.itemId}
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