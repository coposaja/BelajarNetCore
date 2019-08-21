namespace SampleApp
{
    internal static class Markup
    {
        internal const string Text = @"<!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""utf-8"" />
                <title>Key Vault Configuration Provider Sample</title>
                <style>body{{font-family:sans-serif}}div{{overflow-x:auto}}table{{border-collapse:collapse}}th,td{{padding:8px;border-bottom:1px solid black}}th{{background-color:#4CAF50;color:white}}</style>
            </head>
            <body>
                <h1>Key Vault Configuration Provider Sample</h1>
                <div>
                    <table>
                        <thead>
                            <tr>
                                <th>Secret</th>
                                <th>Name in Key Vault</th>
                                <th>Obtained from Configuration</th>
                                <th>Value</th> 
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>SecretName</td>
                                <td><b>SecretName</b></td>
                                <td><code>Configuration[""SecretName""]</code></td>
                                <td>{0}</td> 
                            </tr>
                            <tr>
                                <td rowSpan=""2"">Section:SecretName</td>
                                <td rowSpan=""2""><b>Section--SecretName</b></td>
                                <td><code>Configuration[""Section:SecretName""]</code></td>
                                <td>{1}</td> 
                            </tr>
                            <tr>
                                <td><code>Configuration.GetSection(""Section"")[""SecretName""]</code></td>
                                <td>{2}</td> 
                            </tr>
                        </tbody>
                    </table>
                </div>
            </body>
            </html>";
    }
}
