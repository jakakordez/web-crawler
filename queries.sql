
select count(p.id) from crawldb.page as p ;

select count(p.id) from crawldb.page as p WHERE p.page_type_code != 'FRONTIER';

select count(l.from_page) from crawldb.link as l;

select p.id, p.url from crawldb.page as p;

select * from crawldb.page as p where p.url like '%.docx';

select * from crawldb.page as p where p.id = 104;

select * from crawldb.site;

select * from crawldb.link;

select * from crawldb.page_data;

select * from crawldb.image;


-- vizualizacija:

select p.id, p.url from crawldb.page as p;

select * from crawldb.link;

-- statistika:

select count(s.id) from crawldb.site as s ;
select count(p.id) from crawldb.page as p ;
select count(p.id) from crawldb.page as p WHERE p.page_type_code = 'DUPLICATE';
select count(pd.id) from crawldb.page_data as pd;
select count(pd.id) from crawldb.page_data as pd where pd.data_type_code == 'PDF';
select count(pd.id) from crawldb.page_data as pd where pd.data_type_code == 'DOC';
select count(pd.id) from crawldb.page_data as pd where pd.data_type_code == 'DOCX';
select count(pd.id) from crawldb.page_data as pd where pd.data_type_code == 'PPT';
select count(pd.id) from crawldb.page_data as pd where pd.data_type_code == 'PPTX'; 

select count(i.id) from crawldb.image as i;
-- number of images pre web page...
