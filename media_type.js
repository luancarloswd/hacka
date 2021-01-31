try {
    var params = JSON.parse(value);
    Zabbix.Log(4, '[ Api Hacka ] JSON: ' + params);

    var request = new CurlHttpRequest(),
        facts = [],
        body = params;

    request.AddHeader('Content-Type: application/json');

    if (typeof params.HTTPProxy === 'string' && params.HTTPProxy !== '') {
        request.SetProxy(params.HTTPProxy);
    }

    Zabbix.Log(4, '[ Api Hacka ] JSON: ' + JSON.stringify(body));

    var response = request.Post(params.endpoint, JSON.stringify(body));

    Zabbix.Log(4, '[ Api Hacka ] Response: ' + response);


    return 'OK';
}
catch (error) {
    Zabbix.Log(3, '[ Api Hacka ] ERROR: ' + error);
    throw 'Sending failed: ' + error;
}