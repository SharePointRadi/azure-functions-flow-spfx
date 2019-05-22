declare interface IDocumentArchiveCommandSetStrings {
  Command1: string;
  Command2: string;
}

declare module 'DocumentArchiveCommandSetStrings' {
  const strings: IDocumentArchiveCommandSetStrings;
  export = strings;
}
