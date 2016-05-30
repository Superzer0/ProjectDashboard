'use strict';
app.controller('appsManagerController', ['$scope', 'tokensManagerService', 'notificationService',
    function ($scope, tokensManagerService, notificationService) {

        $scope.refreshTokens = [];
        $scope.clients = [];

        $scope.authClient = {
            id: undefined,
            name: undefined,
            applicationType: "0",
            allowedOrigin: undefined
        };

        $scope.secret = undefined;

        tokensManagerService.getRefreshTokens().then(function (results) {
            $scope.refreshTokens = results.data;
        }, function (error) {
            notificationService.addError("refresh tokens list", "could not fetch refresh tokens");
        });

        $scope.deleteRefreshTokens = function (index, tokenid) {
            tokenid = window.encodeURIComponent(tokenid);
            tokensManagerService.deleteRefreshTokens(tokenid).then(function (results) {
                $scope.refreshTokens.splice(index, 1);
            }, function (error) {
                notificationService.addError("delete refresh token", "action failed " + error.data.message);
            });
        }

        $scope.createApp = function () {
            tokensManagerService.createApp($scope.authClient)
                .then(function (response) {
                    $scope.authClient = response.data;
                    $scope.authClient.applicationType = $scope.authClient.applicationType + '';
                    $scope.secret = $scope.authClient.secret;
                    notificationService.addSuccess("create app", "created " + $scope.authClient.name);
                    $scope.getAllClients();
                },
                    function (error) {
                        notificationService.addError("create app", error.data.message);
                    });
        };

        $scope.getAllClients = function () {
            tokensManagerService.getClients()
                .then(function (results) {
                    $scope.clients = results.data;
                },
                    function (error) {
                        notificationService.addError("clients list", "could not fetch client list");
                    });
        };

        $scope.newAppCreated = function () {
            return $scope.authClient.id;
        };

        $scope.deleteApp = function (index, clientId, clientName) {
            if (confirm('Are your sure to delete this app?')) {

                tokensManagerService.deleteClient(clientId)
                    .then(function () {
                        $scope.clients.splice(index, 1);
                        notificationService.addSuccess("delete client", "deleted " + clientName + " client.");
                    },
                        function () {
                            notificationService.addError("delete client", "could not delete " + clientName + " client.");
                        });
            }
        }

        $scope.toggleClientState = function (index, clientId, clientName, desiredState) {
            tokensManagerService.changeClientStatus(clientId, desiredState)
                .then(function () {
                    var app = $scope.clients[index];
                    if (app && app.client) {
                        app.client.active = desiredState;
                        notificationService.addSuccess("client status", "changed status for " + clientName);
                    }
                },
                function () {
                    notificationService.addSuccess("client status", "could not change status for " + clientName);
                });

        }

        $scope.reGenerateAppId = function (index, clientId, clientName) {
            if (confirm('Are your sure to genereate new app id for this app?')) {
                tokensManagerService.regenerateClientSecret(clientId)
                    .then(function (response) {
                        notificationService.addSuccess("new client secret", response.data);
                    },
                        function () {
                            notificationService.addSuccess("new client secret", "could not generate new secret for " + clientName);
                        });

            }
        }

        $scope.getAllClients();

    }]);