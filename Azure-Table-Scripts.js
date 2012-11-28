/*
*  AZURE MOBILE SERVICES - Data Table Scripts
*  
*   From the dashboard of your mobile service, go to the DATA tab
*   Create each of the following tables, open each table and go to
*   the SCRIPT tab of each table.
*
*   In each table's script section, paste and save the following
*   scripts which correspond to each table operation
*  
*/


//-----------------
// TABLE: WishList
//-----------------

//READ
// NO CHANGE from Default

//INSERT
function insert(item, user, request) {
    item.UserId = user.userId;
    request.execute();
}

//UPDATE
function update(item, user, request) {
    item.UserId = user.userId;
    request.execute();
}

//DELETE
function del(id, user, request) {

    //Need to delete any associated Entries to the list being deleted
    var entryTable = tables.getTable('Entry');
    
    entryTable.where({ListId: id}).read({
       success: function(results) {
          if (results.length > 0) {
              for (var i = 0; i < results.length; i++) {
                  entryTable.del(results[i].id);                  
              }
          }
          
          //Finally, allow the delete of the actual list
          request.execute(); 
       }
   });
}



//-----------------
// TABLE: Entry
//-----------------

//READ
// NO CHANGE from Default

//INSERT
function insert(item, user, request) {
    item.UserId = user.userId;
    request.execute();
}

//UPDATE
function update(item, user, request) {
    item.UserId = user.userId;
    request.execute();
}

//DELETE
function del(id, user, request) {

    var entryTable = tables.getTable('Entry');
    
    entryTable.where({Id: id}).read({
       success: function(results) {
          if (results.length > 0) {
              if (results[0].imageGuid) {
                var imageGuid = results[0].imageGuid;
                deleteEntryImage(id, user, request, imageGuid);
            }
          } 
       }
    });
    
    request.execute();
}

//Deletes a leftover EntryImage for the specified ImageGuid
function deleteEntryImage(id, user, request, imageGuid) {
    var entryImageTable = tables.getTable('EntryImage');
    
    entryImageTable.where({ImageGuid: imageGuid}).read({
        success: function(results) {
            if (results.length > 0) 
                entryImageTable.del(results[0].id);        
        }         
    });
}



//--------------------
// TABLE: EntryImage
//--------------------

//READ
// NO CHANGE from Default

//INSERT
function insert(item, user, request) {
    item.UserId = user.userId;
    request.execute();
}

//UPDATE
function update(item, user, request) {
    item.UserId = user.userId;
    request.execute();
}

//DELETE
// NO CHANGE from Default
