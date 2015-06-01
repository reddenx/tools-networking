var TaskViewModel = function (parentItem) {
    var self = this;

    //data
    self.TaskId = ko.observable();
    self.Parent = parentItem;
    self.Name = ko.observable();
    self.Description = ko.observable();
    self.CurrentStatus = ko.observable();
    self.Children = ko.observableArray([]);

    self.EditModel = {
        EditName: ko.observable(),
        EditDescription: ko.observable(),
        EditStatus: ko.observable()
    }

    //UI states
    self.IsEditing = ko.observable(false);
    self.IsBusy = ko.observable(false);
    self.ShowChildren = ko.observable(false);
    self.HasDescription = ko.computed(function () {
        return self.Description() && self.Description().length > 0;
    });
    self.ShowDescription = ko.observable(false);
    self.ShowingCompleteChildren = ko.observable(false);

    //methods
    self.StartEditing = function () {
        self.IsEditing(true);
    }
    self.StopEditing = function () {
        self.IsEditing(false);

        self.EditModel.EditName(self.Name());
        self.EditModel.EditDescription(self.Description());
        self.EditModel.EditStatus(self.CurrentStatus());
    }
    self.AddNewChild = function () {
        var child = new TaskViewModel(self);
        child.IsEditing(true);

        self.ShowChildren(true);
        self.Children.push(child);
    }
    self.Save = function () {
        self.IsBusy(true);

        $.ajax({
            url: Urls.Ajax.SaveTask,
            type: 'POST',
            dataType: 'json',
            data:
                {
                    TaskId: self.TaskId(),
                    ParentTaskId: self.Parent ? self.Parent.TaskId() : null,
                    TaskName: self.EditModel.EditName(),
                    Description: self.EditModel.EditDescription(),
                    CurrentStatus: self.EditModel.EditStatus(),
                },
            success: function (response) {
                if (response.Success) {
                    self.AssignValuesFromServer(response.Data);
                    self.StopEditing();

                    if (!self.HasDescription()) {
                        self.ShowDescription(false);
                    }
                }
                else {
                    alert('unable to save data');
                }
            },
            error: function (errorData) {
                alert('unable to save data');
            },
            complete: function () {
                self.IsBusy(false);
            }
        });
    }
    self.ShowCompletedChildren = function () { }
    self.HandleCompletedChildrenResponse = function () { }
    self.HideCompletedChildren = function () { }
    self.ToggleChildren = function () {
        self.ShowChildren(!self.ShowChildren());
    }

    self.AssignFromServerValues = function (serverModel) {

        self.AssignValuesFromServer(serverModel.Item);
        
        $.each(serverModel.Children, function (index, item) {
            var taskModel = new TaskViewModel();
            taskModel.AssignFromServerValues(item);
            self.Children.push(taskModel);
        });

        self.StopEditing();
    }

    self.AssignValuesFromServer = function (serverItem) {
        self.TaskId(serverItem.TaskId);
        self.Name(serverItem.Name);
        self.Description(serverItem.Description);
        self.CurrentStatus(serverItem.CurrentStatus);
    }

    return self;
}


//function TreeItem(serverItem, parentViewModel) {
//    var self = this;

//    //Data
//    self.TaskId = ko.observable(serverItem.Item.TaskId);
//    self.ParentTaskId = serverItem.Item.ParentTaskId;

//    self.Name = ko.observable(serverItem.Item.Name);
//    self.Description = ko.observable(serverItem.Item.Description);
//    self.CurrentStatus = ko.observable(serverItem.Item.CurrentStatus);
//    self.DateCreated = ko.observable(serverItem.Item.DateCreated);
//    self.DateCompleted = ko.observable(serverItem.Item.DateCompleted);

//    self.Parent = parentViewModel;
//    self.Children = ko.observableArray();
//    self.Backup = {};

//    //visual 
//    self.ShowingCompleteChildren = ko.observable(false);
//    self.IsEditing = ko.observable(false);
//    self.IsBusy = ko.observable(false);
//    self.Error = ko.observable('');
//    self.ShowChildren = ko.observable(false);
//    self.ShowExpander = ko.computed(function () {
//        return self.Children().length > 0
//            || !!self.Description();
//    });
//    self.ShowDescription = ko.observable(false);

//    //methods
//    self.ResetValues = function (model) {
//        self.TaskId(model.TaskId);
//        self.Name(model.Name);
//        self.Description(model.Description);
//        self.CurrentStatus(model.CurrentStatus);
//        self.DateCreated(model.DateCreated);
//        self.DateCompleted(model.DateCreated);
//        self.Description(model.Description);
//    }

//    self.ToggleChildren = function () {
//        if (self.ShowChildren() && !self.IsEditing()) {
//            self.ShowDescription(false);
//        }

//        self.ShowChildren(!self.ShowChildren());
//    }

//    self.AddChild = function () {
//        var child = new TreeItem({ Item: {}, Children: [] }, self);
//        child.IsEditing(true);
//        child.ParentTaskId = self.TaskId();
//        child.Parent = self;

//        self.ShowChildren(true);
//        self.Children.push(child);
//    }

//    self.Save = function () {
//        self.IsBusy(true);
//        self.Error('');

//        $.ajax({
//            url: 'SaveTask',
//            dataType: 'json',
//            type: 'POST',
//            data: self.GetSaveData(),
//            success: function (result) {
//                if (result.success) {
//                    self.IsEditing(false);
//                    self.ResetValues(result.data);
//                    self.SetBackup(result.data);

//                    //collapse description if it's been emptied
//                    if (self.Description() != null && self.Description().length <= 0) {
//                        self.ShowDescription(false);
//                    }
//                }
//                else {
//                    self.Error(result.error);
//                }
//            },
//            error: function () {
//                self.Error('Could not save');
//            },
//            complete: function () {
//                self.IsBusy(false);
//            }
//        });
//    }

//    self.GetSaveData = function () {
//        return {
//            'TaskId': self.TaskId(),
//            'ParentTaskId': self.ParentTaskId,
//            'TaskName': self.Name(),
//            'Description': self.Description(),
//            'CurrentStatus': self.CurrentStatus(),
//        }
//    }

//    self.CancelEdits = function () {
//        self.ResetValues(self.Backup);
//        self.IsEditing(false);

//        if (!self.Description()) {
//            self.ShowDescription(false);
//        }

//        if (!self.TaskId()) {
//            self.Parent.Children.remove(self);
//        }
//    }

//    self.SetBackup = function (model) {
//        self.Backup.Name = model.Name;
//        self.Backup.TaskId = model.TaskId;
//        self.Backup.Description = model.Description;
//        self.Backup.CurrentStatus = model.CurrentStatus;
//        self.Backup.DateCreated = model.DateCreated;
//        self.Backup.DateCompleted = model.DateCompleted;
//        self.Backup.Description = model.Description;
//    }

//    self.ShowCompletedChildren = function () {
//        self.ShowingCompleteChildren(true);
//        $.ajax({
//            url: 'GetAllChildrenForTask',
//            dataType: 'json',
//            type: 'POST',
//            data: { 'taskId': self.TaskId() },
//            success: function (response) {
//                if (response.success) {
//                    self.HandleCompletedChildrenReply(response.data);
//                }
//                else {
//                    self.ShowingCompleteChildren(false);
//                }
//            },
//            error: function () {
//                self.ShowingCompleteChildren(false);
//            }
//        });
//    }

//    self.HideCompletedChildren = function () {
//        self.Children.remove(function (item) {
//            return item.CurrentStatus() === 2;
//        });

//        self.ShowingCompleteChildren(false);
//    }

//    self.HandleCompletedChildrenReply = function (data) {

//        $.each(data, function (index, item) {
//            var match = ko.utils.arrayFirst(self.Children(), function (childItem) {
//                return childItem.TaskId() === item.Item.TaskId;
//            });

//            if (!match) {
//                self.Children.push(new TreeItem(item, self));
//            }
//        });
//    }

//    //build children
//    $.each(serverItem.Children, function (index, item) {
//        self.Children.push(new TreeItem(item, self));
//    });

//    //setup backup
//    self.SetBackup(serverItem.Item);

//    return self;
//}