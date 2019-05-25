import { override } from '@microsoft/decorators';
import { Log } from '@microsoft/sp-core-library';
import {
    BaseListViewCommandSet,
    Command,
    IListViewCommandSetListViewUpdatedParameters,
    IListViewCommandSetExecuteEventParameters
} from '@microsoft/sp-listview-extensibility';
import { Dialog } from '@microsoft/sp-dialog';

import * as strings from 'DocumentArchiveCommandSetStrings';
import InputPane from './InputPane';

/**
 * If your command set uses the ClientSideComponentProperties JSON input,
 * it will be deserialized into the BaseExtension.properties object.
 * You can define an interface to describe it.
 */
export interface IDocumentArchiveCommandSetProperties {
    // This is an example; replace with your own properties
    sampleTextOne: string;
    sampleTextTwo: string;
}

const LOG_SOURCE: string = 'DocumentArchiveCommandSet';

export default class DocumentArchiveCommandSet extends BaseListViewCommandSet<IDocumentArchiveCommandSetProperties> {

    @override
    public onInit(): Promise<void> {
        Log.info(LOG_SOURCE, 'Initialized DocumentArchiveCommandSet');
        return Promise.resolve();
    }

    @override
    public onListViewUpdated(event: IListViewCommandSetListViewUpdatedParameters): void {
        const compareOneCommand: Command = this.tryGetCommand('COMMAND_1');
        if (compareOneCommand) {
            // This command should be hidden unless exactly one row is selected.
            compareOneCommand.visible = event.selectedRows.length === 1;
        }
    }

    @override
    public onExecute(event: IListViewCommandSetExecuteEventParameters): void {
        switch (event.itemId) {
            case 'COMMAND_1':
                const itemId = event.selectedRows[0].getValueByName("ID");
                const spFilePath = "https://" + window.location.hostname + event.selectedRows[0].getValueByName("FileRef");
                const pane = new InputPane({ httpClient: this.context.httpClient, itemId: itemId, spFilePath: spFilePath });
                pane.show();
                break;
            default:
                throw new Error('Unknown command');
        }
    }
}
